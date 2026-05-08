    namespace Sudoku;

using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using TileValue = ushort;

/*
* Programming 2 - Assignment 3 – Winter 2026
* Created by: Thanpisit Nimprasert / 2566758
*
* Description: A Sudoku game using 2d array as foundation
*/
public class Sudoku
    {
        public enum GameState
        {
            InProgress,
            Player1Won,
            Quit,
            Tie,
            Invalid
        }
        //Global variables
        static int Shuffle = 20, MinNum = 1, MaxNum = 10,DefaultSlot = 10, EmptySlot = DefaultSlot, BlockSize;
        static ConsoleKeyInfo key;
        static ushort boardSize = 9;
        const int DefaultSize = 9, SmallSize = 4;
        static Random rand = new Random();
        static char[] Letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };
        const ushort SudokuPlayer = 0;
        const string EmptyPosition = "";
        const TileValue EmptyValue = 0;
        static int DifficultySlot = DefaultSlot;
        


        /*
         * Starter code to allocate the 2D array for the board
         * and to fill it will a valid completed Sudoku
         */

    // Generate Board if board size is equal to the default size(9x9), draw a normal one(9x9), otherwise draw a small one(4x4), then return it
        public static TileValue[,] GenerateBoard()
        {
            TileValue[,] board = new TileValue[boardSize, boardSize];
            if (boardSize == DefaultSize)
            {
                DefaultSudoku(board);
            }
            else
            {
                SmallSudoku(board);
            }

            return board;
        }
    //Sudoku 9x9, Values
        public static void DefaultSudoku(TileValue[,] board)
        {
            TileValue[,] Board =
            {
                    { 1,2,3,4,5,6,7,8,9 },
                    { 4,5,6,7,8,9,1,2,3 },
                    { 7,8,9,1,2,3,4,5,6 },
                    { 2,3,4,5,6,7,8,9,1 },
                    { 5,6,7,8,9,1,2,3,4 },
                    { 8,9,1,2,3,4,5,6,7 },
                    { 3,4,5,6,7,8,9,1,2 },
                    { 6,7,8,9,1,2,3,4,5 },
                    { 9,1,2,3,4,5,6,7,8 },
                };
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    board[row, col] = Board[row, col];
                }
            }
            ShuffleBoard(board); // Shuffle the board instead of randomize everything
            RemoveCells(board); // remove slots
        }
    // Draw the board(9x9)
        public static void DrawBoardDefault(TileValue[,] board)
        {
            ColumnHeader(); // For Headers 
            for (int row = 0; row < boardSize; row++)
            {
                if (row % BlockSize == 0) // Draw this once the block is finish. ex. every 3 lines for 9x9
                {
                    Centering("  *-------*-------*-------*");
                }
                StringBuilder cursor = new StringBuilder();
                cursor.Append($"{row + 1} ");
                for (int col = 0; col < boardSize; col++)
                {

                    if (col % BlockSize == 0) // Same as above but horizontally
                    {
                        cursor.Append("| ");
                    }
                    if (board[row, col] == 0) // GIVE THEM SOME SPACE!
                    {
                        cursor.Append("  ");
                    }
                    else
                    {
                        cursor.Append($"{board[row, col]} "); // insert number
                    }
                }
                cursor.Append("| ");
                Centering(cursor.ToString());
            }
            Centering("  *-------*-------*-------*"); // to close the box
        }

    // Same as the other one, just smaller
            public static void SmallSudoku(TileValue[,] board) 
            {
                TileValue[,] Board =
                {
                    {1,2,3,4,},
                    {3,4,1,2,},
                    {2,1,4,3 },
                    {4,3,2,1,},

                };
                for (int row = 0; row < boardSize; row++)
                {
                    for (int col = 0; col < boardSize; col++)
                    {
                        board[row, col] = Board[row, col];
                    }
                }
                ShuffleBoard(board);
                RemoveCells(board);
            }
    //Same as the other one, just smaller
            public static void DrawBoardSmall(TileValue[,] board)
            {
                ColumnHeader();
                for (int row = 0; row < boardSize; row++)
                {
                    if (row % BlockSize == 0)
                    {
                        Centering("  *-----*-----*");
                    }
                    StringBuilder cursor = new StringBuilder();
                    cursor.Append($"{row + 1} ");
                    for (int col = 0; col < boardSize; col++)
                    {

                        if (col % BlockSize == 0)
                        {
                            cursor.Append("| ");
                        }
                        if (board[row, col] == 0)
                        {
                            cursor.Append("  ");
                        }
                        else
                        {
                            cursor.Append($"{board[row, col]} ");
                        }
                    }
                    cursor.Append("| ");
                    Centering(cursor.ToString());
                }
                    Centering("  *-----*-----*");
            }
    //Basically header 💔
            static void ColumnHeader()
            {
                StringBuilder cursor = new StringBuilder();
                if (boardSize == DefaultSize)
                {
                    cursor.Append("  ");
                    for (int col = 0; col < boardSize; col++)
                    {
                        if (col == 2 || col == 5) // Make big space every 3 letters
                        {
                            cursor.Append($"{Letters[col]}   ");
                        } // Make small space every letter
                        else
                        {
                            cursor.Append($"{Letters[col]} ");
                        }
                    }
                }
                else
                {
                    cursor.Append("  ");
                    for (int col = 0; col < boardSize; col++)
                    {
                        if (col == 1 || col == 4) //Same as above, for smaller board
                        {
                            cursor.Append($"{Letters[col]}   ");
                        }
                        else
                        {
                            cursor.Append($"{Letters[col]} ");
                        }
                    }
                }
                Centering(cursor.ToString());
            }
    // HERE WE SHUFFLE!
    public static void ShuffleBoard(TileValue[,] board)
    {
        for (int i = 0; i < Shuffle; i++)
        {
            CheckRange(); // To check the range of random/ for next line
            int Num1 = rand.Next(MinNum, MaxNum);
            int Num2 = rand.Next(MinNum, MaxNum);
            if (Num1 == Num2) //To prevent repeated number
            {
                continue;
            }
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (board[row, col] == Num1) // If the board has the same number as randomized, change it to second randomized number
                    {
                        board[row, col] = (ushort)Num2;
                    }
                    else if (board[row, col] == Num2) // If the board has the same number as randomized, change it to first randomized number
                    {
                        board[row, col] = (ushort)Num1;
                    }
                }
            }
        }
    }
    //Check range for random number in the function above
            public static void CheckRange()
            {
                if (boardSize == DefaultSize)
                {
                    return;
                }
                if (boardSize == SmallSize)
                {
                    MaxNum = boardSize + 1; //+1 since max in random is exclusive
                }
            }
        // Remove random cells in the board until it reaches the set number
            public static void RemoveCells(TileValue[,] board)
            {
                int empty = EmptySlot;
                while (empty > 0)
                {
                    int row = rand.Next(0, boardSize);
                    int col = rand.Next(0, boardSize);
                    if (board[row, col] != 0)
                    {
                        board[row, col] = 0;
                        empty--;
                    }
                }
            }
            /* 
             * Starter code to recursively fill the board with random numbers
             * from the set of numbers for this size of sudoku board.
             * When an invalid number is placed, we throw it away and try again.
             * 
             * A set of shuffled (i.e random order) numbers is used to avoid
             * the possibility of repeatedly trying the same "random" number(s)
             * over and over.
             */
            /*static bool FillBoard(TileValue[,] board, ushort boardSize)
            {
                for (int row = 0; row < boardSize; row++)
                {
                    for (int col = 0; col < boardSize; col++)
                    {
                        // Find an empty cell
                        if (board[row, col] == 0)
                        {
                            // Create a shuffled list of numbers to ensure a unique board every time
                            TileValue[] numbers = GetShuffledNumbers((TileValue)boardSize);

                            foreach (TileValue num in numbers)
                            {
                                board[row, col] = num;

                                // Check whether this board is valid or if any rules
                                // were broken trying to add this number into the board
                                if (ValidateBoard(board))
                                {
                                    // the number we added was good, so recursively call the same
                                    // function to try to fill in the version of the board including
                                    // what we just added
                                    if (FillBoard(board, boardSize))
                                    {
                                        return true;
                                    }
                                }

                                // the number we tried adding was not valid, or we were
                                // unable to fill in the board after adding that number so
                                // take out the number we just added and try again with 
                                // another number
                                board[row, col] = 0;
                            }

                            return false; // No valid number found for this cell
                        }
                    }
                }

                // if we get here then we've filled the board and it is valid
                return true;
            }
            */
            /*
            static TileValue[] GetShuffledNumbers(TileValue max)
        {
            // create an array of the numbers 1..max
            TileValue[] nums = new TileValue[max];
            for (TileValue i = 0; i < max; i++)
            {
                nums[i] = (TileValue)(i + 1);
            }
        
            // Fisher-Yates shuffle.
            // See https://en.wikipedia.org/wiki/Fisher–Yates_shuffle
            for (int i = max - 1; i > 0; i--)
            {
                int j = Random.Shared.Next(i + 1);

                // swap the two values:
                TileValue temp = nums[i];
                nums[i] = nums[j];
                nums[j] = temp;
            }

            return nums;
        }


        /*
         * Returns true if the board is valid for Sudoku.
         * Returns false if any rules were broken.
         */
        static bool ValidateBoard(TileValue[,] board, int row, int col, int value)
        {
            for (int c = 0; c < boardSize; c++)
            {
                if (board[row, c] == value) // if the input is the same as what's in that row, return false
                {
                    return false;
                }
            }
            for (int r = 0; r < boardSize; r++) 
            {
                if (board[r, col] == value) // if the input is the same as what's in that column, return false
            {
                    return false;
                }
            }
            int startRow = (row / BlockSize) * BlockSize; // find the top of the block
            int startCol = (col / BlockSize) * BlockSize; // find the left of the block
            for (int r = startRow; r < startRow + BlockSize; r++)
            {
                for (int c = startCol; c < startCol + BlockSize; c++)
                {
                    if (board[r, c] == value) // if the input is repeated with any cells in the block, return false
                { 
                    return false;
                }
                }
            }
            return true; // nothing's wrong, we good
        }


        /*
         * Initializes the program to begin a game of the given size
         * and returns the initial game board
         */

    //Start method for the game
        public static TileValue[,] NewGame(ushort size)
        {
        if (size != DefaultSize && size != SmallSize) // Check if the size is good, if it's not return null| Shouldn't happen due to choosing size using preset button and not freely input
        {
            return null;
        }
        boardSize = size;
        EmptySlot = DifficultySlot; // Reset Emptyslot
        if (size == DefaultSize) // Check Size for test
        {
            BlockSize = 3;
        }
        if (size == SmallSize) // Check Size for test
        {
            BlockSize = 2;
            EmptySlot = EmptySlot / 3; // Reduce empty slot because of smaller board
        }
            TileValue[,] board = GenerateBoard(); // here we go, creating the board

            return board;
        }
    // To determine the game state
    public static void PlayGame(TileValue[,] board)
    {
        if (boardSize  == DefaultSize)
        {
            DrawBoardDefault(board);
        }
        else
        {
            DrawBoardSmall(board);
        }
            GameState state = GameState.InProgress;
        while (state == GameState.InProgress) // If it's in progress, keep asking for move
        {
            state = Move(board, SudokuPlayer, EmptyPosition, EmptyValue);
        }
        if (state == GameState.Quit) // If the player desire to quit, quit💔
        {
            Console.Clear();
            return;
        }
        if (state == GameState.Player1Won) // If the player won, we move on the EndGame
        {
            EndGame(board);
        }
        Console.ReadKey();
        Console.Clear();
    }
    //The player won, but what's after? Save the board | Return to menu
    public static void EndGame(TileValue[,] board)
    {
        string directory = "../../../"; // Same directory as .cs
        Console.WriteLine("Congratulation! You've won!");
        Console.WriteLine("Do you want to save the board? Y/N");
        while (true)
        {
            key = Console.ReadKey(true);
            char UserChar = char.ToUpper(key.KeyChar);
            if (UserChar == 'Y') //YES!!!
            {
                if (SaveGameBoard(board, directory)) //If the file is successfully generated, open it up
                {
                    OpenFile(directory);
                }
                return;
            }
            else if (UserChar == 'N') //NO...
            {
                return;
            }
        }
    }
        /*
         * Applies a player's move to the board if it is legal.
         * player is either 0 or 1 for tic-tac-toe (player 0 goes first so they draw Xs). Player should always be 0 for Sudoku
         * Returns true if the move was accepted or false if the move was rejected
         */

    // Main core of the game, player's choice and literally the whole game
        public static GameState Move(TileValue[,] board, ushort player, string position, TileValue value)
        {
                Console.Clear(); // Clear the old board and draw a new one with user input in it
                if (boardSize == DefaultSize)
                {
                    DrawBoardDefault(board);
                }
                else
                {
                    DrawBoardSmall(board);
                }
                if (IsBoardComplete(board)) // Check if every cells has been filled
                {
                    return GameState.Player1Won;
                }
                Centering("Enter Your Move (ex. A1 4) or Q to quit");
                Console.SetCursorPosition(Console.WindowWidth/2, Console.CursorTop); //Console.CursorTop is like using $ in excel, basically stays where you are
        string UserInput = Console.ReadLine().ToUpper(); // User input and also convert this to uppercase
                if (UserInput == "Q")
            {
                return GameState.Quit;
            }        
                if (UserInput.Length >= 4) // Standard input size ex. A1 1 took 4 spaces, if not then do nothing
                {
                    char ColChar = UserInput[0];
                    ushort RowNum, Value;
                    if (ushort.TryParse(UserInput[1].ToString(), out RowNum) && ushort.TryParse(UserInput[3].ToString(), out Value)) // Validate User input
                    {
                        int row = RowNum - 1; // Match the array with the input, since array start at 0, but header for the board starts at 1, so this reduce it to match the array index
                        int col = ColChar - 'A'; // Same goes here, for Column with character
                        if (ValidateBoard(board, row, col, Value)) // Check if the user input repeated with an existing number or not
                        {
                            board[row, col] = Value; // Put user input in the board
                            return GameState.InProgress;
                        }
                        else //If it's invalid then ask again
                        {
                            Console.WriteLine("Invalid move Press any key to continue"); 
                            Console.ReadKey();
                        }
                }
            }
                return GameState.InProgress;
        }

    // To check if every cells has been filled or not
        static bool IsBoardComplete(TileValue[,] board)
        {
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (board[row, col] == 0) //Empty cell?
                    {
                        return false; // NO
                    }
                }
            }
            return true; // YES
        }


    /*
     * Saves a text version of the game board to the specified directory. The name of the
     * file must be "gameboard.txt"
     */

    //This function belongs to Keith with a few adjustments, used to save the board to a .txt file
    public static bool SaveGameBoard(TileValue[,] board, string directory)
    {
        bool success = false;           // assume it's going to fail somehow
        StreamWriter? sw = null;
        string filePath = $"{directory}gameboard.txt";

        try
        {
            sw = new StreamWriter(filePath);
            sw.Write(GenerateContent(board));
            success = true;             // no exceptions were thrown, so everything worked!
        }
        catch (Exception e)
        {
            Console.WriteLine($"Something went wrong creating the file '{filePath}'.");
            Console.WriteLine(e);
        }
        finally
        {
            if (sw != null)
            {
                sw.Close();
            }
        }

        return success;
    }
    // Generate the content of the txt file
    public static string GenerateContent(TileValue[,] board) // Function is in string cuz it's the content
    {
        StringBuilder sb = new StringBuilder();
        AppendColumnHeaders(sb); // Board header
        AppendBoardRows(sb,board); // Board row
        AppendBorderLine(sb); // Board separation lines
        return sb.ToString(); //Convert it to string to make sure and then return it
    }

    // Append Column header A B C ...
    public static void AppendColumnHeaders(StringBuilder sb)
    {
        sb.Append("    ");
        for (int col = 0; col < boardSize; col++)
        {
            if (boardSize == DefaultSize) // For 9x9
            {
                if (col == 2 || col == 5)
                {
                    sb.Append($"{Letters[col]}   ");
                }
                else
                {
                    sb.Append($"{Letters[col]} ");
                }
            }
            else
            {
                if (col == 1) // For 4x4
                {
                    sb.Append($"{Letters[col]}   ");
                }
                else
                {
                    sb.Append($"{Letters[col]} ");
                }
            }
                
        }
        sb.AppendLine(); // Goes to next line once done
    }

    // Append separation line
    public static void AppendBoardRows(StringBuilder sb, TileValue[,] board)
    {
        for (int row = 0; row < boardSize; row++)
        {
            if (row % BlockSize == 0) // if it needs a horizontal line, draw it, supposedly every 3 numbers for default size
            {
                AppendBorderLine(sb);
            }
                sb.Append($"{row + 1} ");
            for (int col = 0; col < boardSize; col++)
            {
                if (col % BlockSize == 0) // if it needs a vertical line, draw it, supposedly every 3 numbers for default size
                {
                    sb.Append("| ");
                }
                if (board[row, col] == 0) // Fail safe if the slot it's empty, shouldn't happen
                {
                    sb.Append("  ");
                }
                else // If it doesn't needs anything and it's number, append it in
                {
                    sb.Append($"{board[row, col]} "); 
                }
                }
                sb.AppendLine("| "); // For the start and the end of the board
            }
        }
    // Draw separation line, horizontally
    public static void AppendBorderLine(StringBuilder sb)
    {
        if (boardSize == DefaultSize)
        {
            sb.AppendLine("  *-------*-------*-------*");
        }
        else
        { 
            sb.AppendLine("  *-----*-----*");
        }
    }
    //Open file, once again belongs to Keith with a few adjustment
    public static void OpenFile(string directory)
    {
        try
        {
            // Process.Start doesn't support relative paths so we need to get the full "absolute" path:
            string filePath = Path.GetFullPath($"{directory}gameboard.txt");

            ProcessStartInfo psi = new ProcessStartInfo(filePath);
            psi.UseShellExecute = true;

            Process.Start(psi);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Something went wrong opening the file '{directory}'.");
            Console.WriteLine(e);
        }
    }
    //Yep, This is Main, very tiny
        static void Main(string[] args)
        {
        Console.CursorVisible = false;
            MainMenu();
        }
    // Main Menu, prompt user his/her choice and instruction
        public static void MainMenu()
        {
        InstructionPrompt();
        Console.ReadKey();
        Console.Clear();
            while (true)
            {
                MenuPrompt();
                ConsoleKeyInfo key = Console.ReadKey(true);
                char UserChar = key.KeyChar;
                if (UserChar == '1' || UserChar == '2' || UserChar == '3')
                {
                    switch (key.KeyChar)
                    {
                        case '1':
                            Console.Clear();
                            TileValue[,] board = NewGame(boardSize); //Create a new board everytime user decides to start, prevent board repeating
                            PlayGame(board); //Here we play
                            break;
                        case '2':
                            Console.Clear();
                            Settings();// go to setting
                            break;
                        case '3':
                            Console.Clear();
                            return;
                    }
                }
                else //If user pressed anything else except the specified option, nothing will happen
                {
                    Console.Clear();
                    continue;
                }
            }
        }
    // Setting for user experience
        public static void Settings()
        {
            while (true)
            {
                SettingPrompt();
                ConsoleKeyInfo key = Console.ReadKey(true);
                char UserChar = key.KeyChar;
                if (key.KeyChar == '1' || key.KeyChar == '2' || key.KeyChar == '3' || key.KeyChar == '4')
                {
                    switch (key.KeyChar)
                    {
                        case '1':
                            Console.Clear();
                            boardSize = 9;
                            return;
                        case '2':
                            Console.Clear();
                            boardSize = 4;
                            return;
                        case '3':
                            Console.Clear();
                            Difficulty();
                            return;
                    case '4':
                        Console.Clear();
                        return;
                    }

                }
                else //If user pressed anything else except the specified option, nothing will happen
                {
                    Console.Clear();
                    continue;
                }
        }
        }
    //Difficulty to be determined here, default is medium
    public static void Difficulty()
    {
        while (true)
        {
            DifficultyPrompt();
            ConsoleKeyInfo key = Console.ReadKey(true);
            char UserChar = key.KeyChar;
            if (key.KeyChar == '1' || key.KeyChar == '2' || key.KeyChar == '3')
            {
                switch (key.KeyChar)
                {
                    case '1':
                        Console.Clear();
                        DifficultySlot = DefaultSlot / 2;
                        return;
                    case '2':
                        Console.Clear();
                        DifficultySlot = DefaultSlot;
                        return;
                    case '3':
                        Console.Clear();
                        DifficultySlot = DefaultSlot * 2;
                        return;
                }
            }
            else //If user pressed anything else except the specified option, nothing will happen
            {
                Console.Clear();
                continue;
            }
        }
    }
    //UI Improvement
    public static void Centering(string line)
    {
        int padding = (Console.WindowWidth - line.Length) / 2;
        if (padding < 0)
        {
            padding = 0;
        }
        Console.SetCursorPosition(padding, Console.CursorTop); //Console.CursorTop is like using $ in excel, basically stays where you are
        Console.WriteLine(line);
    }
    // Bunch of prompt to reduce lines used in function
    public static void MenuPrompt()
        {
            Centering("   _____           _       _          \r\n  / ____|         | |     | |         \r\n | (___  _   _  __| | ___ | | ___   _ \r\n  \\___ \\| | | |/ _` |/ _ \\| |/ / | | |\r\n  ____) | |_| | (_| | (_) |   <| |_| |\r\n |_____/ \\__,_|\\__,_|\\___/|_|\\_\\\\__,_|\r\n                                      \r\n                                      ");
            Console.WriteLine();
            Centering("1. Start");
            Console.WriteLine();
            Centering("2. Settings");
            Console.WriteLine();
            Centering("3. Quit");


        }
       public static void SettingPrompt()
        {
            Centering("   _____      _   _   _                 \r\n  / ____|    | | | | (_)                \r\n | (___   ___| |_| |_ _ _ __   __ _ ___ \r\n  \\___ \\ / _ \\ __| __| | '_ \\ / _` / __|\r\n  ____) |  __/ |_| |_| | | | | (_| \\__ \\\r\n |_____/ \\___|\\__|\\__|_|_| |_|\\__, |___/\r\n                               __/ |    \r\n                              |___/     ");
            Console.WriteLine();
            Centering("1. Sudoku Size: 9x9 (Default)");
            Console.WriteLine();
            Centering("2. Sudoku Size: 4x4");
            Console.WriteLine();
            Centering("3. Difficulty");
            Console.WriteLine();
            Centering("4. Return to Main Menu");

    }
    public static void DifficultyPrompt()
    {
        Centering("  _____  _  __  __ _            _ _         \r\n |  __ \\(_)/ _|/ _(_)          | | |        \r\n | |  | |_| |_| |_ _  ___ _   _| | |_ _   _ \r\n | |  | | |  _|  _| |/ __| | | | | __| | | |\r\n | |__| | | | | | | | (__| |_| | | |_| |_| |\r\n |_____/|_|_| |_| |_|\\___|\\__,_|_|\\__|\\__, |\r\n                                       __/ |\r\n                                      |___/ ");
        Console.WriteLine();
        Centering("1. Easy");
        Console.WriteLine();
        Centering("2. Medium");
        Console.WriteLine();
        Centering("3. Hard");
    }
    public static void InstructionPrompt()
    {
        Centering("  _____ _   _  _____ _______ _____  _    _  _____ _______ _____ ____  _   _ \r\n |_   _| \\ | |/ ____|__   __|  __ \\| |  | |/ ____|__   __|_   _/ __ \\| \\ | |\r\n   | | |  \\| | (___    | |  | |__) | |  | | |       | |    | || |  | |  \\| |\r\n   | | | . ` |\\___ \\   | |  |  _  /| |  | | |       | |    | || |  | | . ` |\r\n  _| |_| |\\  |____) |  | |  | | \\ \\| |__| | |____   | |   _| || |__| | |\\  |\r\n |_____|_| \\_|_____/   |_|  |_|  \\_\\\\____/ \\_____|  |_|  |_____\\____/|_| \\_|\r\n                                                                            \r\n                                                                            ");
        Centering("Welcome to Suduko!!!");
        Centering("In this game, you will have to fill the empty cells with numbers 1-9 for default size and 1-4 for small size");
        Centering("Each row must have all numbers without repeating");
        Centering("Each column must have all numbers without repeating");
        Centering("Each box must have all numbers without repeating");
        Centering("Complete the board to win");
        Console.WriteLine();
        Centering("Are you ready? Press any key to start!");
    }
}
