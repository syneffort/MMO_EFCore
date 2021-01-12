using System;

namespace MMO_EFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            DbCommands.InitializeDB(forceReset: false);

            // CRUD
            Console.WriteLine("명령어를 입력하세요");
            Console.WriteLine("[0] Force reset");
            Console.WriteLine("[1] Test");

            while (true)
            {
                Console.Write("> ");
                string command = Console.ReadLine().ToLower();
                switch (command)
                {
                    case "0":
                        DbCommands.InitializeDB(forceReset: true);
                        break;
                    case "1":
                        DbCommands.Test();
                        break;
                    case "2":
                        break;
                    case "3":
                        break;
                    case "exit":
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}
