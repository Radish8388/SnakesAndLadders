// must install NuGet Package: System.Speech
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Numerics;
using System.Reflection.Emit;
using System.Speech.Synthesis;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakesAndLadders
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random _random = new Random();
        DispatcherTimer Timer;
        Stopwatch _pauseTimer = new Stopwatch();
        SpeechSynthesizer synth;
        Image _board = new Image();
        Token _playerToken = new Token();
        Token _computerToken = new Token();
        bool _tokensChosen = false;
        double _leftMargin = 0;
        double _topMargin = 0;
        double _squareSize = 10;
        int _startSquare = 0;
        int _endSquare = 0;
        int _currentSquare = 0;
        int _nextSquare = 0;
        bool _isPlayersTurn = false;
        Image? _imageToMove;
        int _roll = 0;
        double _playerOffset = -0.2;
        double _computerOffset = 0.2;
        double _destX, _destY;
        double _stepX, _stepY;
        bool _isPausing = false;
        bool _isSliding = false;
        bool _isSoundOn = true;

        public MainWindow()
        {
            InitializeComponent();

            // set up DispatcherTimer
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 frames per second
            Timer.Tick += Timer_Tick;

            // set up SpeechSynthesizer
            synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            // Set the volume (0-100)
            synth.Volume = 100;
            // Set the speed (-10 to 10)
            synth.Rate = -2;
            synth.SelectVoiceByHints(VoiceGender.Female);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double screenWidth = SystemParameters.WorkArea.Width;
            double screenHeight = SystemParameters.WorkArea.Height;

            // ensure window size doesn't exceed screen size
            if (this.Width > screenWidth) this.Width = screenWidth;
            if (this.Height > screenHeight) this.Height = screenHeight;

            // ensure window is not off the left or top
            if (this.Left < 0) this.Left = 0;
            if (this.Top < 0) this.Top = 0;

            // ensure window is not off the right or bottom
            if (this.Left + this.Width > screenWidth)
                this.Left = screenWidth - this.Width;
            if (this.Top + this.Height > screenHeight)
                this.Top = screenHeight - this.Height;

            string filename = "pack://application:,,,/images/board.png";
            _board.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
            Table.Children.Add(_board);

            //NewGame();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Debug.WriteLine($"window size = {this.Width}x{this.Height}");
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void Dice_Click(object sender, RoutedEventArgs e)
        {
            _roll = _random.Next(1, 7);
            //_roll = 4;
            PrintAndSpeak($"You rolled a {_roll}.");
            DiceButton.IsEnabled = false;
            ContinuePlayerTurn();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_isPausing)
            {
                if (_pauseTimer.ElapsedMilliseconds > 2000)
                    _isPausing = false;
            }
            if (!_isPausing)
            {
                double left = Canvas.GetLeft(_imageToMove);
                double top = Canvas.GetTop(_imageToMove);

                //Debug.WriteLine($"L-X={left-destX}, dx={stepX}");
                //Debug.WriteLine($"T-Y={top - destY}, dy={stepY}");
                if ((Math.Abs(left - _destX) <= Math.Abs(_stepX)) && (Math.Abs(top - _destY) <= Math.Abs(_stepY)))
                // if very close to destination
                {
                    Timer.Stop();
                    EndMove();
                }
                else
                {
                    left += _stepX;
                    top += _stepY;
                    Canvas.SetLeft(_imageToMove, left);
                    Canvas.SetTop(_imageToMove, top);
                }
            }
        }

        private void NewGame()
        {
            DiceButton.IsEnabled = false;
            if (!_tokensChosen) ChooseTokens();
            _playerToken.MoveTo(0);
            //_playerToken.MoveTo(90);
            _computerToken.MoveTo(0);

            // set token positions
            MoveToPosition(_playerToken, _playerToken.Square, _playerOffset);
            MoveToPosition(_computerToken, _computerToken.Square, _computerOffset);

            if (_random.Next(2) == 0)
                StartPlayerTurn();
            else
                StartComputerTurn();
        }

        private void ChooseTokens()
        {
            //Dispatcher.BeginInvoke(new Action(() =>
            //{
                int playerTokenColor = 0;
                int computerTokenColor = 0;

                // call dialog to choose player's token
                ChooseToken dlg = new ChooseToken();
                dlg.Owner = this;
                bool? result = dlg.ShowDialog();
                if (result == true) // if player chose a color
                    playerTokenColor = dlg.ChosenToken;
                else // if player closed the dialog without choosing
                    playerTokenColor = _random.Next(1, 5);

                // choose random computer token
                computerTokenColor = _random.Next(1, 4);
                if (computerTokenColor == playerTokenColor)
                    computerTokenColor++;
                _tokensChosen = true;

                // set top row colors
                switch (computerTokenColor)
                {
                    case 1:
                        MeText.Foreground = Brushes.Green;
                        string filename = "pack://application:,,,/images/greenToken.png";
                        MeToken.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        _computerToken.Img.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        break;
                    case 2:
                        MeText.Foreground = Brushes.Blue;
                        filename = "pack://application:,,,/images/blueToken.png";
                        MeToken.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        _computerToken.Img.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        break;
                    case 3:
                        MeText.Foreground = Brushes.Purple;
                        filename = "pack://application:,,,/images/purpleToken.png";
                        MeToken.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        _computerToken.Img.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        break;
                    case 4:
                        MeText.Foreground = Brushes.Gray;
                        filename = "pack://application:,,,/images/grayToken.png";
                        MeToken.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        _computerToken.Img.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        break;
                }

                // set top row colors
                switch (playerTokenColor)
                {
                    case 1:
                        YouText.Foreground = Brushes.Green;
                        string filename = "pack://application:,,,/images/greenToken.png";
                        YouToken.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        _playerToken.Img.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        break;
                    case 2:
                        YouText.Foreground = Brushes.Blue;
                        filename = "pack://application:,,,/images/blueToken.png";
                        YouToken.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        _playerToken.Img.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        break;
                    case 3:
                        YouText.Foreground = Brushes.Purple;
                        filename = "pack://application:,,,/images/purpleToken.png";
                        YouToken.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        _playerToken.Img.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        break;
                    case 4:
                        YouText.Foreground = Brushes.Gray;
                        filename = "pack://application:,,,/images/grayToken.png";
                        YouToken.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        _playerToken.Img.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        break;
                }

                Table.Children.Add(_playerToken.Img);
                Table.Children.Add(_computerToken.Img);
            //}));
        }

        private void Table_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double tw = Table.ActualWidth;
            double th = Table.ActualHeight;

            // set board size
            double boardSize = Math.Min(tw, th);
            _board.Width = boardSize;
            _board.Height = boardSize;

            // set left margin
            _leftMargin = 0;
            if (_board.Width < tw)
            {
                _leftMargin = (tw - _board.Width) / 2.0;
                Canvas.SetLeft(_board, _leftMargin);
            }
            _leftMargin += (15 / 726.0) * boardSize;

            // set top margin
            _topMargin = 0;
            if (_board.Height < th)
            {
                _topMargin = (th - _board.Height) / 2.0;
                Canvas.SetTop(_board, _topMargin);
            }
            _topMargin += (15 / 726.0) * boardSize;

            // set square size
            _squareSize = (69.6 / 726.0) * boardSize;

            // set token sizes
            _playerToken.Size = _squareSize / 2.0;
            _playerToken.Img.Width = _playerToken.Size;
            _playerToken.Img.Height = _playerToken.Size;
            _computerToken.Size = _squareSize / 2.0;
            _computerToken.Img.Width = _computerToken.Size;
            _computerToken.Img.Height = _computerToken.Size;

            // set token positions
            MoveToPosition(_playerToken, _playerToken.Square, _playerOffset);
            MoveToPosition(_computerToken, _computerToken.Square, _computerOffset);
        }

        private void MoveToPosition(Token token, int square, double offset)
        {
            int row = token.GetRow(square);
            int col = token.GetCol(square);
            double y = _topMargin + (row + 0.5) * _squareSize - token.Size / 2.0;
            double x = _leftMargin + (col + 0.5) * _squareSize - token.Size / 2.0 + token.Size * offset;
            Canvas.SetLeft(token.Img, x);
            Canvas.SetTop(token.Img, y);
        }

        private double GetX(Token token, int square, double offset)
        {
            int col = token.GetCol(square);
            double x = _leftMargin + (col + 0.5) * _squareSize - token.Size / 2.0 + token.Size * offset;
            return x;
        }

        private double GetY(Token token, int square, double offset)
        {
            int row = token.GetRow(square);
            double y = _topMargin + (row + 0.5) * _squareSize - token.Size / 2.0;
            return y;
        }

        private void StartPlayerTurn()
        {
            if (CheckEndOfGame()) return;
            _isPlayersTurn = true;
            PrintAndSpeak("Press the dice button.");
            DiceButton.IsEnabled = true;
            _isSliding = false;
        }

        private void ContinuePlayerTurn()
        {
            _startSquare = _playerToken.Square;
            _endSquare = _startSquare + _roll;
            _currentSquare = _startSquare;
            _nextSquare = _currentSquare + 1;
            _imageToMove = _playerToken.Img;
            _isPausing = true;
            _pauseTimer.Restart();
            StartNextMove();
        }

        private void StartComputerTurn()
        {
            if (CheckEndOfGame()) return;
            _isPlayersTurn = false;
            _roll = _random.Next(1, 7);
            PrintAndSpeak($"I rolled a {_roll}.");
            _isSliding = false;

            _startSquare = _computerToken.Square;
            _endSquare = _startSquare + _roll;
            _currentSquare = _startSquare;
            _nextSquare = _currentSquare + 1;
            _imageToMove = _computerToken.Img;
            _isPausing = true;
            _pauseTimer.Restart();
            StartNextMove();
        }

        private void StartNextMove()
        {
            double startX, startY;
            Canvas.SetZIndex(_imageToMove, 2);

            if (_isPlayersTurn)
            {
                startX = GetX(_playerToken, _currentSquare, _playerOffset);
                startY = GetY(_playerToken, _currentSquare, _playerOffset);
                _destX = GetX(_playerToken, _nextSquare, _playerOffset);
                _destY = GetY(_playerToken, _nextSquare, _playerOffset);
            }
            else
            {
                startX = GetX(_computerToken, _currentSquare, _computerOffset);
                startY = GetY(_computerToken, _currentSquare, _computerOffset);
                _destX = GetX(_computerToken, _nextSquare, _computerOffset);
                _destY = GetY(_computerToken, _nextSquare, _computerOffset);
            }
            double dx = _destX - startX;
            double dy = _destY - startY;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            _stepX = dx * 0.1 * _squareSize / distance;
            _stepY = dy * 0.1 * _squareSize / distance;

            Timer.Start();
        }

        private void EndMove()
        {
            Canvas.SetZIndex(_imageToMove, 1);
            if (_isPlayersTurn)
            {
                _playerToken.MoveTo(_nextSquare);
                MoveToPosition(_playerToken, _playerToken.Square, _playerOffset);
            }
            else
            {
                _computerToken.MoveTo(_nextSquare);
                MoveToPosition(_computerToken, _computerToken.Square, _computerOffset);
            }

            if (!_isSliding && _nextSquare < _endSquare) // not done moving
            {
                _currentSquare++;
                _nextSquare++;
                StartNextMove();
            }
            else // finished moving for this dice roll
            {
                _currentSquare = _endSquare;
                _nextSquare = _playerToken.SlideTo(_endSquare);
                if (!_isSliding && _nextSquare > _endSquare) // stopped at the bottom of a ladder
                {
                    _isPausing = true;
                    _pauseTimer.Restart();
                    PrintAndSpeak("Climbing up!");
                    _isSliding = true;
                    StartNextMove();
                }
                else if (!_isSliding && _nextSquare < _endSquare) // stopped at the top of a snake
                {
                    _isPausing = true;
                    _pauseTimer.Restart();
                    PrintAndSpeak("Sliding down!");
                    _isSliding = true;
                    StartNextMove();
                }
                else if (_roll == 6) // rolling a 6 get an extra turn
                {
                    if (_isPlayersTurn)
                        StartPlayerTurn();
                    else
                        StartComputerTurn();
                }
                else
                {
                    if (_isPlayersTurn)
                        StartComputerTurn();
                    else
                        StartPlayerTurn();
                }
            }
        }

        private bool CheckEndOfGame()
        {
            if (_playerToken.Square == 100)
            {
                PrintAndSpeak("Congratulations, you won!");
                return true;
            }
            else if (_computerToken.Square == 100)
            {
                PrintAndSpeak("Sorry, you lost.");
                return true;
            }
            return false;
        }

        private void PrintAndSpeak(string text)
        {
            Instructions.Text = text;
            SpeakText(text);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            NewGame();
        }

        private void SpeakText(string text)
        {
            synth.SpeakAsyncCancelAll();
            // Speak synchronously (freezes UI) or asynchronously (better for WPF)
            if (_isSoundOn) synth.SpeakAsync(text);
        }

        private void SoundButton_Click(object sender, RoutedEventArgs e)
        {
            _isSoundOn = !_isSoundOn;
            if (_isSoundOn)
                SoundImage.Source = new BitmapImage(new Uri("/images/soundOn.png", UriKind.Relative));
            else
            {
                SoundImage.Source = new BitmapImage(new Uri("/images/soundOff.png", UriKind.Relative));
                synth.SpeakAsyncCancelAll();
            }
        }
    }
}