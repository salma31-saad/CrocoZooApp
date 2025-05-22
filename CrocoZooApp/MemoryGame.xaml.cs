using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CrocoZooApp.DAO;
using CrocoZooApp.CrocoZooDB;
using Microsoft.VisualBasic.ApplicationServices;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Diagnostics;

namespace CrocoZooApp
{
    public partial class MemoryGame : Window
    {
        // Liste des animaux pour les cartes
        private List<string> animalImages = new List<string>
        {
            "🐶", "🐱", "🐭", "🐰", "🦊", "🐻", "🐼", "🐨", "🦁", "🐯", "🐮", "🐷", "🐸", "🐵", "🐔", "🦄"
        };

        private List<Button> cards = new List<Button>();
        private Button firstSelectedCard = null;
        private Button secondSelectedCard = null;
        private bool isProcessing = false;
        private int matchesFound = 0;
        private int totalPairs = 0;
        private DispatcherTimer gameTimer;
        private TimeSpan elapsedTime;
        private string currentDifficulty = "Difficile"; // Par défaut

        // Dictionnaire pour stocker les paires d'animaux
        private Dictionary<string, string> cardPairs = new Dictionary<string, string>();

        public MemoryGame()
        {
            InitializeComponent();

            // Initialiser le timer
            gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            gameTimer.Tick += Timer_Tick;

            // Désactiver le jeu jusqu'à ce que le joueur choisisse une difficulté
            GameGrid.IsEnabled = false;
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null && clickedButton.Tag != null)
            {
                currentDifficulty = clickedButton.Tag.ToString();
            }

            // Réinitialiser le jeu
            ResetGame();

            // Configurer le jeu selon la difficulté
            SetupGameByDifficulty(currentDifficulty);

            // Activer le jeu - CORRECTION ICI
            GameGrid.IsEnabled = true;

            // Masquer les boutons de difficulté
            difficultyBox.Visibility = Visibility.Collapsed;

            // Démarrer le timer
            elapsedTime = TimeSpan.Zero;
            gameTimer.Start();

            // Changer le texte du bouton de démarrage
            StartGame.Visibility = Visibility.Collapsed;
        }

        private void SetupGameByDifficulty(string difficulty)
        {
            // Réinitialiser la grille
            foreach (UIElement element in GameGrid.Children)
            {
                if (element is Button button)
                {
                    button.Visibility = Visibility.Collapsed;
                    button.IsEnabled = true; // Réactiver toutes les cartes

                    // Supprimer les gestionnaires d'événements existants
                    button.Click -= Card_Click;

                    // Réinitialiser le Tag
                    button.Tag = null;

                    // Réinitialiser le contenu visuel des cartes
                    string cardName = button.Name;
                    if (!string.IsNullOrEmpty(cardName) && cardName.StartsWith("Card_"))
                    {
                        // Réinitialiser le dos de la carte
                        string backName = "CardBack" + cardName.Substring(4);
                        Border cardBack = FindName(backName) as Border;
                        if (cardBack != null)
                        {
                            cardBack.Visibility = Visibility.Visible;
                        }

                        // Réinitialiser l'image de la carte
                        string imageName = "CardImage" + cardName.Substring(4);
                        Image cardImage = FindName(imageName) as Image;
                        if (cardImage != null)
                        {
                            cardImage.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }

            cards.Clear();
            cardPairs.Clear();

            int rows = 0;
            int columns = 0;
            Color cardColor = Colors.Purple;
            int pairsCount = 0;

            // Configurer selon la difficulté
            switch (difficulty)
            {
                case "Facile":
                    rows = 2;
                    columns = 3;
                    pairsCount = 3;
                    totalPairs = 3;
                    cardColor = (Color)ColorConverter.ConvertFromString("#D64541"); // Rouge
                    break;
                case "Moyen":
                    rows = 3;
                    columns = 4;
                    pairsCount = 6;
                    totalPairs = 6;
                    cardColor = (Color)ColorConverter.ConvertFromString("#F39C12"); // Orange
                    break;
                case "Difficile":
                default:
                    rows = 4;
                    columns = 4;
                    pairsCount = 8;
                    totalPairs = 8;
                    cardColor = (Color)ColorConverter.ConvertFromString("#9B59B6"); // Violet
                    break;
            }

            // Mettre à jour l'affichage du score
            ScoreText.Text = $"0/{totalPairs}";

            // CORRECTION: Amélioration de la création des paires
            // Mélanger la liste d'animaux pour avoir des paires différentes à chaque partie
            List<string> shuffledAnimals = new List<string>(animalImages);
            Random random = new Random();
            shuffledAnimals = shuffledAnimals.OrderBy(x => random.Next()).ToList();

            // Sélectionner les premiers animaux selon le nombre de paires nécessaires
            List<string> selectedAnimals = shuffledAnimals.Take(pairsCount).ToList();

            // Créer les paires (chaque animal apparaît deux fois)
            List<string> gamePairs = new List<string>();
            foreach (string animal in selectedAnimals)
            {
                gamePairs.Add(animal);
                gamePairs.Add(animal);
            }

            // Mélanger les paires
            gamePairs = gamePairs.OrderBy(x => random.Next()).ToList();

            // Configurer les cartes visibles
            int pairIndex = 0;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (pairIndex < gamePairs.Count)
                    {
                        string cardName = $"Card_{row}_{col}";
                        Button card = FindName(cardName) as Button;

                        if (card != null)
                        {
                            // Configurer la carte
                            card.Visibility = Visibility.Visible;
                            card.IsEnabled = true;
                            card.Click += Card_Click;
                            cards.Add(card);

                            // Configurer l'arrière de la carte
                            Border cardBack = FindName($"CardBack_{row}_{col}") as Border;
                            if (cardBack != null)
                            {
                                cardBack.Background = new SolidColorBrush(cardColor);
                                cardBack.Visibility = Visibility.Visible;
                            }

                            // Obtenir l'animal pour cette carte
                            string animal = gamePairs[pairIndex];

                            // CORRECTION: Utiliser une approche différente pour afficher les émojis
                            // Trouver la grille parent dans le bouton
                            Grid parentGrid = null;

                            if (card.Content is Grid grid)
                            {
                                parentGrid = grid;
                            }

                            if (parentGrid != null)
                            {
                                // Créer un nouveau TextBlock pour l'emoji
                                TextBlock animalText = new TextBlock
                                {
                                    Text = animal,
                                    FontSize = 48,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    Visibility = Visibility.Hidden
                                };

                                // Supprimer tous les TextBlocks existants
                                List<UIElement> toRemove = new List<UIElement>();
                                foreach (UIElement child in parentGrid.Children)
                                {
                                    if (child is TextBlock)
                                    {
                                        toRemove.Add(child);
                                    }
                                }

                                foreach (UIElement element in toRemove)
                                {
                                    parentGrid.Children.Remove(element);
                                }

                                // Ajouter le nouveau TextBlock
                                parentGrid.Children.Add(animalText);

                                // Stocker la référence au TextBlock dans le Tag du bouton
                                card.Tag = new Tuple<string, TextBlock>(animal, animalText);
                            }

                            pairIndex++;
                        }
                    }
                }
            }

            // Activer la grille
            GameGrid.IsEnabled = true;
        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {

            if (isProcessing)
                return;

            Button clickedCard = sender as Button;

            // Vérifier si la carte est déjà retournée ou déjà associée
            if (clickedCard == firstSelectedCard || !clickedCard.IsEnabled)
                return;

            // Récupérer les informations de la carte
            var cardInfo = clickedCard.Tag as Tuple<string, TextBlock>;
            if (cardInfo == null)
            {
                // Si le Tag n'est pas correctement configuré, essayons de le récupérer
                if (clickedCard.Content is Grid grid)
                {
                    TextBlock foundText = null;
                    foreach (UIElement child in grid.Children)
                    {
                        if (child is TextBlock)
                        {
                            foundText = child as TextBlock;
                            break;
                        }
                    }

                    if (foundText != null)
                    {
                        // Créer un nouveau Tag avec un emoji par défaut
                        clickedCard.Tag = new Tuple<string, TextBlock>(foundText.Text, foundText);
                        cardInfo = clickedCard.Tag as Tuple<string, TextBlock>;
                    }
                }

                if (cardInfo == null)
                {
                    // Si nous ne pouvons toujours pas récupérer les informations, ignorer ce clic
                    return;
                }
            }

            string currentAnimalEmoji = cardInfo.Item1;
            TextBlock cardImage = cardInfo.Item2;

            // Trouver le dos de la carte
            string backName = "CardBack" + clickedCard.Name.Substring(4); // Remplacer "Card" par "CardBack"
            Border cardBack = FindName(backName) as Border;

            // Retourner la carte (montrer l'animal)
            if (cardBack != null)
                cardBack.Visibility = Visibility.Hidden;

            if (cardImage != null)
            {
                cardImage.Visibility = Visibility.Visible;
            }

            if (firstSelectedCard == null)
            {
                // Première carte sélectionnée
                firstSelectedCard = clickedCard;
            }
            else
            {
                // Deuxième carte sélectionnée
                secondSelectedCard = clickedCard;
                isProcessing = true;

                // Vérifier si les cartes correspondent
                var firstCardInfo = firstSelectedCard.Tag as Tuple<string, TextBlock>;

                if (firstCardInfo != null && firstCardInfo.Item1 == currentAnimalEmoji)
                {
                    // Correspondance trouvée
                    matchesFound++;
                    UpdateScoreDisplay();

                    // Appliquer une animation de correspondance
                    ApplyMatchAnimation(firstSelectedCard);
                    ApplyMatchAnimation(secondSelectedCard);

                    // Désactiver les cartes correspondantes
                    firstSelectedCard.IsEnabled = false;
                    secondSelectedCard.IsEnabled = false;

                    // Réinitialiser la sélection
                    firstSelectedCard = null;
                    secondSelectedCard = null;
                    isProcessing = false;

                    // Vérifier si le jeu est terminé
                    if (matchesFound == totalPairs)
                    {
                        GameCompleted();
                    }
                }
                else
                {
                    // Pas de correspondance, retourner les cartes après un délai
                    DispatcherTimer timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(1)
                    };
                    timer.Tick += (s, args) =>
                    {
                        timer.Stop();

                        // Retourner la première carte
                        string firstBackName = "CardBack" + firstSelectedCard.Name.Substring(4);
                        Border firstBack = FindName(firstBackName) as Border;
                        if (firstBack != null)
                            firstBack.Visibility = Visibility.Visible;

                        var firstInfo = firstSelectedCard.Tag as Tuple<string, TextBlock>;
                        if (firstInfo != null && firstInfo.Item2 != null)
                            firstInfo.Item2.Visibility = Visibility.Hidden;

                        // Retourner la deuxième carte
                        if (cardBack != null)
                            cardBack.Visibility = Visibility.Visible;

                        if (cardImage != null)
                            cardImage.Visibility = Visibility.Hidden;

                        // Réinitialiser la sélection
                        firstSelectedCard = null;
                        secondSelectedCard = null;
                        isProcessing = false;
                    };
                    timer.Start();
                }
            }
        }

      

        private void ApplyMatchAnimation(Button button)
        {
            // Trouver le dos de la carte
            string cardName = button.Name;
            string backName = "CardBack" + cardName.Substring(4);
            Border cardBack = FindName(backName) as Border;

            if (cardBack != null)
            {
                // Changer la couleur en vert pour indiquer une correspondance
                cardBack.Background = new SolidColorBrush(Colors.LightGreen);
                cardBack.Visibility = Visibility.Visible;

                // Animation de pulsation
                DoubleAnimation pulseAnimation = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.8,
                    Duration = TimeSpan.FromSeconds(0.3),
                    AutoReverse = true,
                    RepeatBehavior = new RepeatBehavior(2)
                };

                button.RenderTransform = new ScaleTransform(1, 1);
                button.RenderTransformOrigin = new Point(0.5, 0.5);
                button.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, pulseAnimation);
                button.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, pulseAnimation);

                // Rendre l'image visible par-dessus le fond vert
                var cardInfo = button.Tag as Tuple<string, TextBlock>;
                if (cardInfo != null && cardInfo.Item2 != null)
                {
                    cardInfo.Item2.Visibility = Visibility.Visible;
                    cardInfo.Item2.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        // Ajout d'une méthode pour déboguer les paires
        private void DebugPrintCards()
        {
            Debug.WriteLine("État actuel des cartes:");
            foreach (Button card in cards)
            {
                var cardInfo = card.Tag as Tuple<string, TextBlock>;
                if (cardInfo != null)
                {
                    Debug.WriteLine($"Carte: {card.Name}, Animal: {cardInfo.Item1}, Visible: {cardInfo.Item2.Visibility}");
                }
                else
                {
                    Debug.WriteLine($"Carte: {card.Name}, Tag non configuré");
                }
            }
        }

        private void UpdateScoreDisplay()
        {
            ScoreText.Text = $"{matchesFound}/{totalPairs}";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            TimerText.Text = string.Format("{0:00}:{1:00}", Math.Floor(elapsedTime.TotalMinutes), elapsedTime.Seconds);
        }

        private void GameCompleted()
        {
            gameTimer.Stop();

            // Afficher un message de félicitations
            MessageBox.Show($"Félicitations! Vous avez gagné en {elapsedTime.Minutes} minutes et {elapsedTime.Seconds} secondes!",
                "Partie terminée", MessageBoxButton.OK, MessageBoxImage.Information);

            // Réactiver les options de difficulté
            difficultyBox.Visibility = Visibility.Visible;
            StartGame.Visibility = Visibility.Visible;
        }

        private void ResetGame()
        {
            // Arrêter le timer
            gameTimer.Stop();

            // Réinitialiser les variables
            firstSelectedCard = null;
            secondSelectedCard = null;
            isProcessing = false;
            matchesFound = 0;

            // Réinitialiser l'affichage
            ScoreText.Text = "0/0";
            TimerText.Text = "00:00";

            // Réinitialiser toutes les cartes
            foreach (Button card in cards)
            {
                // Supprimer les gestionnaires d'événements
                card.Click -= Card_Click;

                // Réinitialiser l'état visuel
                if (card.Name.StartsWith("Card_"))
                {
                    // Réinitialiser le dos de la carte
                    string backName = "CardBack" + card.Name.Substring(4);
                    Border cardBack = FindName(backName) as Border;
                    if (cardBack != null)
                    {
                        cardBack.Visibility = Visibility.Visible;
                    }

                    // Cacher l'image/texte
                    var cardInfo = card.Tag as Tuple<string, TextBlock>;
                    if (cardInfo != null && cardInfo.Item2 != null)
                    {
                        cardInfo.Item2.Visibility = Visibility.Hidden;
                    }

                    // Réactiver la carte
                    card.IsEnabled = true;
                }
            }

            cards.Clear();
        }

        private void StartNewGame(object sender, RoutedEventArgs e)
        {
            // Réinitialiser le jeu
            ResetGame();

            // Réactiver les options de difficulté
            difficultyBox.Visibility = Visibility.Visible;
            StartGame.Visibility = Visibility.Visible;

            // Désactiver le jeu jusqu'à ce que le joueur choisisse une difficulté
            GameGrid.IsEnabled = false;
        }
    }


}
