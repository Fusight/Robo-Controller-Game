﻿using Engine;
using Robo_Controller_Game.Properties;
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

namespace Robo_Controller_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameController gameController;

        public MainWindow()
        {
            InitializeComponent();

            gameController = new GameController((int)GameBoard.Width / 10, (int)GameBoard.Height / 10, GameBoard, CPUProgressBar, this);
            ShowRobotEquipment();
        }

        private void ShowRobotEquipment()
        {
            //Clear
            robotPartGrid.Children.Clear();
            robotPartGrid.RowDefinitions.Clear();
            robotPartGrid.ColumnDefinitions.Clear();
            //Setup
            int c = 0;
            foreach (RobotEquipment part in gameController.activeEquipment)
            {
                RowDefinition row = new RowDefinition
                {
                    Height = new GridLength(140)
                };
                robotPartGrid.RowDefinitions.Add(row);
                RobotPart robotPart = new RobotPart(part.name, part.Description, part.image)
                {
                    Name = part.id
                };
                Grid.SetColumn(robotPart, 0);
                Grid.SetRow(robotPart, c++);
                robotPartGrid.Children.Add(robotPart);
            }

            //Show button to open shop if possible
            if (gameController.toBuy.Count != 0)
            {
                RowDefinition eRow = new RowDefinition
                {
                    Height = new GridLength(75) // Add a little for margin and border!
                };
                Button addPart = new Button
                {
                    Height = 65,
                    Width = 300,
                    Content = "Buy a new part!",
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(3),
                    BorderBrush = Brushes.DarkOliveGreen,
                    BorderThickness = new Thickness(2),
                    Background = Brushes.DarkGray,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                addPart.Click += OpenShop;
                robotPartGrid.RowDefinitions.Add(eRow);
                Grid.SetColumn(addPart, 0);
                Grid.SetRow(addPart, c);
                robotPartGrid.Children.Add(addPart);
            }
        }

        private void OpenShop(object sender, EventArgs e)
        {
            //Clear the grid
            robotPartGridSecond.Children.Clear();
            robotPartGridSecond.RowDefinitions.Clear();
            robotPartGridSecond.ColumnDefinitions.Clear();
            //Display
            int c = 0;
            gameController.toBuy.ForEach(i =>
            {
                robotPartGridSecond.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(140) });
                RobotPart robotPart = new RobotPart(i.name, i.Description, i.image);
                Grid.SetColumn(robotPart, 0);
                Grid.SetRow(robotPart, c++);
                robotPart.Click += ShopClick;
                robotPart.Name = i.id;
                robotPartGridSecond.Children.Add(robotPart);
            });
        }

        private void ShopClick(object sender, EventArgs e)
        {
            string id = ((Control)sender).Name;
            RobotEquipment bought = gameController.toBuy.Find(t => t.id == id);
            //TODO: Handle money
            gameController.toBuy.Remove(bought);
            gameController.activeEquipment.Add(bought);
            //Refresh
            OpenShop(sender, e);
            ShowRobotEquipment();
        }

        private void GameBoard_Initialized(object sender, EventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gameController.RunCode(codeView.Text, out string Error);
            if (Error != "") MessageBox.Show(Error);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void ClearCode_Click(object sender, RoutedEventArgs e)
        {
            codeView.Text = "";
            codeView.Focus();
        }
    }
}