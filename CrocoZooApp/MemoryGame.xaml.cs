using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CrocoZooApp.CrocoZooDB;
using CrocoZooApp.DAO;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;

namespace CrocoZooApp
{
    public partial class MemoryGame : UserControl
    {
        private List<MemoryCard> cards;
        private MemoryCard firstCard, secondCard;
        private DispatcherTimer gameTimer;
        private TimeSpan timeElapsed;
        private int pairsFound;
        private int totalPairs;
        private Random rand = new Random();
        private Grid gameBoard;

        public MemoryGame()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += Timer_Tick;
            timeElapsed = TimeSpan.Zero;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timeElapsed = timeElapsed.Add(TimeSpan.FromSeconds(1));
            TimerText.Text = timeElapsed.ToString(@"mm\:ss");
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            // Trouver l'élément Border sélectionné, en utilisant la source d'événement
            var selectedBorder = sender as Border;
            if (selectedBorder == null)
            {
                MessageBox.Show("Veuillez sélectionner une difficulté.");
                return;
            }

            // Récupérer le TextBlock qui contient le nom du niveau
            var textBlock = selectedBorder.FindName("TextBlock") as TextBlock;
            if (textBlock == null)
            {
                MessageBox.Show("Erreur de sélection du niveau.");
                return;
            }

            // Extraire le texte du TextBlock pour le niveau sélectionné
            string level = textBlock.Text;
            SetupBoard(level);
            StartNewGame();
        }


        private void SetupBoard(string difficulty)
        {
            switch (difficulty)
            {
                case "Facile":
                    CreateBoard(4, 4); break;
                case "Moyen":
                    CreateBoard(6, 6); break;
                case "Difficile":
                    CreateBoard(8, 8); break;
            }
        }

        private void CreateBoard(int rows, int cols)
        {
            gameBoard = GameGrid;
            gameBoard.Children.Clear();
            gameBoard.RowDefinitions.Clear();
            gameBoard.ColumnDefinitions.Clear();

            for (int i = 0; i < rows; i++)
                gameBoard.RowDefinitions.Add(new RowDefinition());

            for (int j = 0; j < cols; j++)
                gameBoard.ColumnDefinitions.Add(new ColumnDefinition());

            cards = new List<MemoryCard>();
            totalPairs = (rows * cols) / 2;
            pairsFound = 0;
            ScoreText.Text = "Paires trouvées : 0";
            TimerText.Text = "00:00";
            timeElapsed = TimeSpan.Zero;
            gameTimer.Start();

            var images = LoadAnimalImages();
            var selectedImages = images.OrderBy(x => rand.Next()).Take(totalPairs).ToList();
            var cardImages = selectedImages.Concat(selectedImages).OrderBy(x => rand.Next()).ToList();

            int index = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var card = new MemoryCard { ImagePath = cardImages[index++] };
                    var btn = CreateCardButton(card);
                    Grid.SetRow(btn, row);
                    Grid.SetColumn(btn, col);
                    gameBoard.Children.Add(btn);
                    card.Button = btn;
                    cards.Add(card);
                }
            }
        }

        private Button CreateCardButton(MemoryCard card)
        {
            var btn = new Button();
            btn.Click += Card_Click;
            btn.Tag = card;
            btn.Content = CreateImage("/Images/back.png");
            return btn;
        }

        private Image CreateImage(string path)
        {
            return new Image
            {
                Source = new BitmapImage(new Uri(path, UriKind.Relative)),
                Stretch = System.Windows.Media.Stretch.Uniform
            };
        }

        private List<string> LoadAnimalImages()
        {
            // Remplace par tes vrais chemins d’images
            return new List<string>
            {
                "/Images/lion.png",
                "/Images/tigre.png",
                "/Images/elephant.png",
                "/Images/zebra.png",
                "/Images/singe.png",
                "/Images/girafe.png",
                "/Images/hippo.png",
                "/Images/crocodile.png",
                "/Images/ours.png",
                "/Images/koala.png",
                "/Images/renard.png",
                "/Images/loup.png",
                "/Images/panda.png",
                "/Images/rhino.png",
                "/Images/autruche.png",
                "/Images/aigle.png",
                "/Images/cheval.png",
                "/Images/vache.png",
                "/Images/cochon.png",
                "/Images/chien.png"
            };
        }

        private async void Card_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var card = button.Tag as MemoryCard;

            if (card.IsFlipped || secondCard != null)
                return;

            card.IsFlipped = true;
            button.Content = CreateImage(card.ImagePath);

            if (firstCard == null)
            {
                firstCard = card;
            }
            else
            {
                secondCard = card;
                await Task.Delay(800); // Laisse le temps de voir

                if (firstCard.ImagePath == secondCard.ImagePath)
                {
                    pairsFound++;
                    ScoreText.Text = $"Paires trouvées : {pairsFound}";
                    firstCard = secondCard = null;

                    if (pairsFound == totalPairs)
                    {
                        gameTimer.Stop();
                        MessageBox.Show($"Bravo ! Vous avez gagné en {timeElapsed:mm\\:ss}.");
                    }
                }
                else
                {
                    firstCard.IsFlipped = secondCard.IsFlipped = false;
                    firstCard.Button.Content = CreateImage("/Images/back.png");
                    secondCard.Button.Content = CreateImage("/Images/back.png");
                    firstCard = secondCard = null;
                }
            }
        }

        private void StartNewGame()
        {
            firstCard = null;
            secondCard = null;
            gameTimer.Stop();
            timeElapsed = TimeSpan.Zero;
            TimerText.Text = "00:00";
        }
    }

    public class MemoryCard
    {
        public string ImagePath { get; set; }
        public bool IsFlipped { get; set; }
        public Button Button { get; set; }
    }


}
