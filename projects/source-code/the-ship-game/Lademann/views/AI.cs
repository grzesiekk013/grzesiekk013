using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lademann.views
{
    public class AI
    {
        public static class Bot_GameLobbyView
        {
            public static int total_amount_ships = 10;
            public static int ships_size_one = 4;
            public static int ships_sieze_two = 3;
            public static int ships_size_three = 2;
            public static int ships_size_four = 0;
            public static int[,] ships_position = new int[10, 10];
        }
        public static class Bot_Game_plan
        {
            public static int total_amount_ships = 20;
            public static int ships_destroyed = 0;
            public static int[,] ships_destroyed_position = new int[10, 10];
            public static bool[,] bot_ships_position = new bool[10, 10];
            public static int ships_size_one = 4;
            public static int ships_sieze_two = 3;
            public static int ships_size_three = 2;
            public static int ships_size_four = 1;
            public static bool up_checked = false;
            public static bool right_checked = false;
            public static bool right_right_checked = false;
            public static bool right_left_checked = false;
            public static bool left_left_checked = false;
            public static bool left_right_checked = false;
            public static bool up_up_checked = false;
            public static bool up_down_checked = false;
            public static bool down_checked = false;           
            public static bool down_down_checked = false;
            public static bool down_up_checked = false;
            public static bool left_checked = false;
            public static bool[,] bot_moves = new bool[100, 100];
            public static bool[,] bot_good_shots_position = new bool[100, 100];
            public static int amount_bot_good_shots = 0;
            public static int total_amount_shots = 0;
            public static bool shooted = false;
            public static bool second_shooted = false;
            public static bool third_shooted = false;
            public static bool fourth_shooted = false;
            public static int last_good_shot_x = 0;
            public static int last_good_shot_y = 0;
            public static int second_good_shot_y_right = 0;
            public static int second_good_shot_y_left = 0;
            public static int second_good_shot_y = 0;
            public static int second_good_shot_x = 0;
            public static int second_good_shot_x_down = 0;
            public static int second_good_shot_x_up = 0;
            public static int third_good_shot_x = 0;
            public static int third_good_shot_y = 0;
            public static int third_good_shot_x_down = 0;
            public static int third_good_shot_x_up = 0;
            public static int third_good_shot_y_right = 0;
            public static bool right_right_shooted = false;
            public static bool right_left_shooted = false;
            public static bool left_right_shooted = false;
            public static bool left_left_shooted = false;
            public static bool up_up_shooted = false;
            public static bool up_down_shooted = false;
            public static bool down_down_shooted = false;
            public static bool down_up_shooted = false;
            public static int third_good_shot_y_left = 0;
            public static int fourth_good_shot_x = 0;
            public static int fourth_good_shot_x_down = 0;
            public static int fourth_good_shot_x_up = 0;
            public static int fourth_good_shot_y = 0;
            public static int fourth_good_shot_y_right = 0;
            public static int fourth_good_shot_y_left = 0;
            public static bool right_shooted = false;
            public static bool left_shooted = false;
            public static bool up_shooted = false;
            public static bool down_shooted = false;
            public static bool fourth_shot_right_right_right_checked = false;
            public static bool fourth_shot_right_right_left_checked = false;
            public static bool fourth_shot_right_left_left_checked = false;
            public static bool fourth_shot_left_left_left_checked = false;
            public static bool fourth_shot_left_left_right_checked = false;
            public static bool fourth_shot_left_right_right_checked = false;
            public static bool fourth_shot_up_up_up_checked = false;
            public static bool fourth_shot_up_up_down_checked = false;
            public static bool fourth_shot_up_down_down_checked = false;
            public static bool fourth_shot_down_down_up_checked = false;
            public static bool fourth_shot_down_down_down_checked = false;
            public static bool fourth_shot_down_up_up_checked = false;

        }

        public bool[,] GenerateMatrix()
        {
            ShipOrientation shipOrientation = ShipOrientation.Horizontal;
            bool[,] shipCords = new bool[10, 10];
            int[] shipsOfNSizePlaced = new int[4];
            int currentShipSize = 1;



            bool[,] matrixx = new bool[10, 10];



            bool placeships(int line, int row)
            {
                if (shipOrientation == ShipOrientation.Vertical)
                {
                    for (int i = 0; i < currentShipSize; ++i)
                    {
                        // najpierw sprawdz czy nie wyjdzie poza boundsy, potem czy nie koliduje z innym statkiem
                        int l = line + i;

                        if (!isInBounds(l) || shipCords[l, row])
                        {
                            return false;
                        }

                        // sprawdz czy na polach dookola (sciany dluzsze) nie ma statku

                        int row_to_check = row + 1;
                        if (isInBounds(row_to_check) && shipCords[l, row_to_check]) // sprawdza pola na prawo od statku
                        {
                            return false;
                        }

                        row_to_check = row - 1;
                        if (isInBounds(row_to_check) && shipCords[l, row_to_check]) // sprawdza pola na lewo od statku
                        {
                            return false;
                        }
                    }

                    // sprawdz dwa pola - pod statkiem i nad

                    int line_to_check = line + currentShipSize;
                    if (isInBounds(line_to_check) && shipCords[line_to_check, row]) // sprawdza pole pod statkiem
                    {
                        return false;
                    }

                    line_to_check = line - 1;
                    if (isInBounds(line_to_check) && shipCords[line_to_check, row]) // sprawdza pole nad statkiem
                    {
                        return false;
                    }

                    for (int i = 0; i < currentShipSize; ++i)
                    {
                        int l = line + i;
                        shipCords[l, row] = true;
                        matrixx[l, row] = true;
                    }
                }
                else if (shipOrientation == ShipOrientation.Horizontal)
                {
                    for (int i = 0; i < currentShipSize; ++i)
                    {
                        int r = row + i;

                        if (!isInBounds(r) || shipCords[line, r])
                        {
                            return false;
                        }

                        int line_to_check = line + 1;
                        if (isInBounds(line_to_check) && shipCords[line_to_check, r]) // sprawdza pola pod statkiem
                        {
                            return false;
                        }

                        line_to_check = line - 1;
                        if (isInBounds(line_to_check) && shipCords[line_to_check, r]) // sprawdza pola nad
                        {
                            return false;
                        }
                    }

                    int row_to_check = row + currentShipSize;
                    if (isInBounds(row_to_check) && shipCords[line, row_to_check]) // sprawdza pole na prawo od statku
                    {
                        return false;

                    }

                    row_to_check = row - 1;
                    if (isInBounds(row_to_check) && shipCords[line, row_to_check]) // sprawdza pole na lewo
                    {

                        return false;
                    }

                    // zaktualizuj matrix i ustaw kolory buttonow
                    for (int i = 0; i < currentShipSize; ++i)
                    {
                        int r = row + i;
                        shipCords[line, r] = true;
                        matrixx[line, r] = true;
                    }
                }
                return true;
            }

            int shipsPlaced = 0;
            //  Console.WriteLine("hello\n");
            shipOrientation = ShipOrientation.Vertical;
            currentShipSize = 4;
            while (true)
            {
                if (GenerateRandomNumber(0, 1) == 0)
                {
                    shipOrientation = ShipOrientation.Horizontal;
                }
                else
                {
                    shipOrientation = ShipOrientation.Vertical;
                }
                if (placeships(GenerateRandomNumber(0, 9), GenerateRandomNumber(0, 9)))
                {
                    shipsPlaced++;
                }

                if (shipsPlaced == 1)
                {

                    currentShipSize = 3;
                    Bot_GameLobbyView.ships_size_three--;
                    Bot_Game_plan.ships_size_three++;

                }
                if (shipsPlaced == 3)
                {

                    currentShipSize = 2;
                    Bot_GameLobbyView.ships_sieze_two--;
                    Bot_Game_plan.ships_sieze_two++;
                }
                if (shipsPlaced == 6)
                {
                    currentShipSize = 1;
                    Bot_GameLobbyView.ships_size_one--;
                    Bot_Game_plan.ships_size_one++;
                }

                if (shipsPlaced == 10) break;


            }

            bool isInBounds(int i)
            {
                return i < 10 && i >= 0;
            }
            return matrixx;
        }

        private static int GenerateRandomNumber(int minValue, int maxValue)
        {
            Random random = new Random(); return random.Next(minValue, maxValue + 1);
        }
        enum ShipOrientation : byte
        {
            Vertical,
            Horizontal,
        }


    }
}

