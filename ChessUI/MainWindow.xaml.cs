using FontAwesome6.Svg;
using Global;
using Logica;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ChessUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChessGame game;
        private Piece SelectedPiece;
        private bool canCancel;
        private bool pauseTask;
        private Task task;
        public MainWindow()
        {
            InitializeComponent();
            game = new ChessGame();
            Draw();

            StartThread();
        }

        private static void WindowsMessage(string message)
        {
            MessageBox.Show(message);
        }

        private void Draw()
        {
            ChessBoardCanvas.Children.Clear();
            DrawChessBoard();
            ArrangeBoard();
        }

        private void CanvasLeftDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(ChessBoardCanvas);
            Coordinates coordinates = new Coordinates((int)point.X, (int)point.Y);

            Position position = game.CoordinatesToPosition(coordinates);
            Tile tile = game.GetTile(position);

            if (game.IsEmptyTile(position) && SelectedPiece != null && tile.IsValidMove)
            {
                SelectedPiece.Position = position;
                SelectedPiece.Moved = true;
                game.ResetMoves();
                Draw();

                game.AiGoodMove();
                game.changeActiveColor();
            }
            else if (!game.IsEmptyTile(position) && SelectedPiece != null && tile.IsValidMove)
            {
                bool team = (bool)game.IsTeamTile(position, SelectedPiece);
                if (team)
                {
                    game.ResetMoves();
                    Piece piece = game.GetPiece(coordinates);

                    Action<string> messageTarget;

                    if (!piece.Color.Equals(game.ActiveColor))
                    {
                        messageTarget = s => WindowsMessage(s);
                        messageTarget("Hello, World!");
                        return;
                    }

                    messageTarget = s => Console.WriteLine(s);

                    game.ShowPossibleMoves(piece);
                    SelectedPiece = piece;
                    return;
                }

                if (game.TakeOver(SelectedPiece, coordinates))
                {
                    SelectedPiece = null;
                    game.changeActiveColor();
                }

            }
            else if (SelectedPiece == null && !game.IsEmptyTile(position))
            {
                Piece piece = game.GetPiece(coordinates);

                if (!piece.Color.Equals(game.ActiveColor))
                    return;

                game.ShowPossibleMoves(piece);
                SelectedPiece = piece;
            }
            else
            {

                SelectedPiece = null;
                game.ResetMoves();
            }

            Draw();
        }

        private void StartThread()
        {
            task = new Task(() =>
            {
                try
                {
                    bool run = true;
                    while (run)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            if (!pauseTask)
                            {
                                if (game.WhiteWin)
                                {
                                    ShowMessage("GameOver", "White won the game");
                                    game = new ChessGame();
                                    Draw();
                                }
                                if (game.BlackWin)
                                {
                                    ShowMessage("GameOver", "Black won the game");
                                    game = new ChessGame();
                                    Draw();
                                }
                                if (game.Stop)
                                {
                                    run = false;
                                }
                                if (game.Draw)
                                {
                                    Draw();
                                    game.Draw = false;
                                }
                                if (game.Load)
                                {
                                    this.Cursor = Cursors.Wait;
                                }
                                else
                                {
                                    this.Cursor = Cursors.Arrow;
                                }
                            }

                        });

                        Thread.Sleep(200);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    this.Dispatcher.Invoke(() =>
                    {
                        ShowMessage("Error", ex.Message);
                    });
                }

            });
            task.Start();
        }

        private void ShowMessage(string head, string text)
        {
            MessageBox.Show(text, head, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ArrangeBoard()
        {
            List<Piece> pieces = game.GetPieces();
            foreach (Piece piece in pieces)
            {
                ImageAwesome icon = DrawChessPieces(game.GetPieceFromName(piece), piece.Color, 30);
                Coordinates coordinates = game.PositionToCoordinates(piece.Position);
                Canvas.SetLeft(icon, coordinates.X);
                Canvas.SetTop(icon, coordinates.Y);

                ChessBoardCanvas.Children.Add(icon);
            }
        }

        private ImageAwesome DrawChessPieces(ChessPieces piece, ChessColor color, int size)
        {
            Brush brush = Brushes.Black;
            if (color == ChessColor.WHITE)
                brush = Brushes.AntiqueWhite;
            if (SelectedPiece != null && SelectedPiece.Equals(piece))
                brush = Brushes.Yellow;

            FontAwesome6.EFontAwesomeIcon icon = FontAwesome6.EFontAwesomeIcon.Regular_CircleXmark;
            switch (piece)
            {
                case ChessPieces.ROOK: icon = FontAwesome6.EFontAwesomeIcon.Solid_ChessRook; break;
                case ChessPieces.BISHOP: icon = FontAwesome6.EFontAwesomeIcon.Solid_ChessBishop; break;
                case ChessPieces.PAWN: icon = FontAwesome6.EFontAwesomeIcon.Solid_ChessPawn; break;
                case ChessPieces.QUEEN: icon = FontAwesome6.EFontAwesomeIcon.Solid_ChessQueen; break;
                case ChessPieces.KNIGHT: icon = FontAwesome6.EFontAwesomeIcon.Solid_ChessKnight; break;
                case ChessPieces.KING: icon = FontAwesome6.EFontAwesomeIcon.Solid_ChessKing; break;
            }

            ImageAwesome imageAwesome = new ImageAwesome
            {
                Width = size,
                Height = size,
                PrimaryColor = brush,
            };

            if (piece != ChessPieces.NONE)
                imageAwesome.Icon = icon;

            return imageAwesome;
        }

        private void DrawChessBoard()
        {
            List<Tile> tiles = game.GetTiles();
            foreach (Tile tile in tiles)
            {
                Rectangle rectangle = TileStyle(tile);
                ChessBoardCanvas.Children.Add(rectangle);

                if (tile.IsValidMove)
                {
                    Ellipse ellipse = TileCircle(tile);
                    ChessBoardCanvas.Children.Add(ellipse);
                }

            }
        }

        private Ellipse TileCircle(Tile tile)
        {
            Ellipse ellipse;
            if (!game.IsEmptyTile(tile))
            {
                ellipse = new Ellipse
                {
                    Width = 40,
                    Height = 40,
                    StrokeThickness = 3,
                    Stroke = Brushes.LightGray
                };

                Canvas.SetTop(ellipse, game.PositionToCoordinates(tile.Position).Y - 5);
                Canvas.SetLeft(ellipse, game.PositionToCoordinates(tile.Position).X - 5);
            }
            else
            {
                ellipse = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    Fill = Brushes.LightGray,
                    Stroke = Brushes.LightGray
                };
                Canvas.SetTop(ellipse, game.PositionToCoordinates(tile.Position).Y + 5);
                Canvas.SetLeft(ellipse, game.PositionToCoordinates(tile.Position).X + 5);
            }


            return ellipse;
        }
        private Rectangle TileStyle(Tile tile)
        {
            ChessColor color = tile.Color;
            Brush brush = Brushes.SaddleBrown;
            if (color == ChessColor.WHITE)
                brush = Brushes.SandyBrown;

            Rectangle rectangle = new Rectangle
            {
                Width = 40,
                Height = 40,
                Fill = brush,
            };
            Canvas.SetTop(rectangle, game.PositionToCoordinates(tile.Position).Y - 5);
            Canvas.SetLeft(rectangle, game.PositionToCoordinates(tile.Position).X - 5);

            return rectangle;
        }

        private void PauseButton(object sender, RoutedEventArgs e)
        {
            pauseTask = !pauseTask;
            if (pauseTask)
            {
                PlayPauseButtonIcon.Icon = FontAwesome6.EFontAwesomeIcon.Regular_CirclePlay;
                PlayPauseButtonText.Text = "PLAY";
            }
            else
            {
                PlayPauseButtonIcon.Icon = FontAwesome6.EFontAwesomeIcon.Regular_CirclePause;
                PlayPauseButtonText.Text = "PAUSE";
            }
        }

        private async void WindowCloseEvent(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debug.WriteLine("Exiting");
            game.Stop = true;
            await task;
            await game.thread;
            ShowMessage("Exit", "Exiting, please wait a few seconds, this will close automatically");
        }
    }
}
