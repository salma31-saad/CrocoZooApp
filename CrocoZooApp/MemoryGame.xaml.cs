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

namespace CrocoZooApp
{
    public partial class MemoryGame : Window
    {
        private List<MemoryCard> cards;
        private MemoryCard firstCard, secondCard;
        private DispatcherTimer gameTimer;
        private TimeSpan timeElapsed;
        private int pairsFound;
        private int totalPairs;
        private Random rand = new Random();

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

        // Boutons avec Tag="Facile", "Moyen", "Difficile"
        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string difficulty)
            {
               
                SetupBoard(difficulty);
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une difficulté.");
            }
        }

        private void SetupBoard(string difficulty)
        {
            switch (difficulty)
            {
                case "Facile": CreateBoard(2, 3); break;  // 3 paires
                case "Moyen": CreateBoard(3, 4); break;   // 6 paires
                case "Difficile": CreateBoard(4, 4); break; // 8 paires
                default: MessageBox.Show("Difficulté inconnue."); break;
            }
        }

        private void CreateBoard(int rows, int cols)
        {

            GameGrid.Children.Clear();
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < rows; i++)
                GameGrid.RowDefinitions.Add(new RowDefinition());

            for (int j = 0; j < cols; j++)
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());

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
                    GameGrid.Children.Add(btn);
                    card.Button = btn;
                    cards.Add(card);
                }
            }
        }

        private Button CreateCardButton(MemoryCard card)
        {
            var btn = new Button
            {
                Tag = card,
                Content = CreateImage("/Images/back.png")
            };
            btn.Click += Card_Click;
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
            return new List<string>
            {
                "C:\\Users\\SLAB70\\OneDrive - Saint Michel\\projets CrocoZoo\\poule.jfif",
                "C:\\Users\\SLAB70\\OneDrive - Saint Michel\\projets CrocoZoo\\vache.png",
                "C:\\Users\\SLAB70\\OneDrive - Saint Michel\\projets CrocoZoo\\lion.jfif",
                "C:\\Users\\SLAB70\\OneDrive - Saint Michel\\projets CrocoZoo\\elephant.jfif",
                "C:\\Users\\SLAB70\\OneDrive - Saint Michel\\projets CrocoZoo\\crocodile.jfif",
                "C:\\Users\\SLAB70\\OneDrive - Saint Michel\\projets CrocoZoo\\chien.jfif",
                "C:\\Users\\SLAB70\\OneDrive - Saint Michel\\projets CrocoZoo\\chat.jfif",
                "C:\\Users\\SLAB70\\OneDrive - Saint Michel\\projets CrocoZoo\\cheval.jpg",
            };
        }

        private async void Card_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not MemoryCard card)
                return;

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
                await Task.Delay(800);

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
                    firstCard.IsFlipped = false;
                    secondCard.IsFlipped = false;
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

        private void StartNewGame(object sender, RoutedEventArgs e)
        {
            StartNewGame(); // Appelle la version sans paramètre
        }
    }

    public class MemoryCard
    {
        public string ImagePath { get; set; }
        public bool IsFlipped { get; set; }
        public Button Button { get; set; }
    }
}
