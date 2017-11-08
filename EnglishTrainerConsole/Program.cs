using EnglishTrainer.Application;
using EnglishTrainer.Domain;
using EnglishTrainer.Infrastructure;
using System;
using System.Collections.Generic;

namespace EnglishTrainerConsole
{
    class Program
    {
        static InMemoryUserRepository UserRepository = new InMemoryUserRepository();
        static InMemoryWordRepository WordRepository =new InMemoryWordRepository();
        static TrainerService TrainerService = new TrainerService();
        UserService UserService = new UserService (UserRepository);
        ViewerService ViewerService;
        ExamineService ExamineService;


        static void Main(string[] args)
        {
            User User;
            Program program = new Program();
            
            while (true) {
                bool isLogin = false;
                while (!isLogin)
                {
                    Console.WriteLine("Нажмите 1 для регистрации и 2 для логина");
                    int answerNumber = program.GetConsoleInt();
                    if (answerNumber == 1)
                    {
                        Console.WriteLine("Введите Имя");
                        User = program.Register(program.GetConsoleString());
                        TrainerService.SaveMemoryUserRepository(UserRepository);
                        isLogin = true;
                        program.SetUp(User);
                    }
                    if (answerNumber == 2)
                    {
                        try
                        {
                            Console.WriteLine("Введите Имя");
                            User = program.LogIn(program.GetConsoleString());
                            isLogin = true;
                            program.SetUp(User);
                        }
                        catch (Exception exeption)
                        {
                            isLogin = false;
                            Console.WriteLine("Такого пользователя не существует");
                        }
                    }
                }
                while (true) {
                    Console.WriteLine("Введите 1 для изучения слов и 2 для просмотра списков слов и 0 для выхода");
                    var answerNumberUser = program.GetConsoleInt();
                    if (answerNumberUser == 1)
                    {
                        
                        try
                        {
                            var twoWords = program.GetTwoWords();
                            Console.WriteLine(twoWords[0].WordInEnglish + " " + "-" + " " + twoWords[1].WordInRussian);
                            Console.WriteLine("Введите 1 если верно и 2 если не верно");
                            var answerNumberTrainer = program.GetConsoleInt();
                            if (answerNumberTrainer == 1)
                            {
                                if (program.Check(true, twoWords))
                                {
                                    Console.WriteLine("Верно");
                                    TrainerService.SaveMemoryUserRepository(UserRepository);
                                }
                                else
                                {
                                    Console.WriteLine("Неверно");
                                    TrainerService.SaveMemoryUserRepository(UserRepository);
                                }
                            }
                            else
                            {
                                if (program.Check(false, twoWords))
                                {
                                    Console.WriteLine("Верно");
                                    TrainerService.SaveMemoryUserRepository(UserRepository);
                                }
                                else
                                {
                                    Console.WriteLine("Неверно");
                                    TrainerService.SaveMemoryUserRepository(UserRepository);
                                }
                            }
                        }
                        catch (Exception exeption)
                        {
                            Console.WriteLine("Вы изучили все слова");
                        }
                        
                    }
                    if (answerNumberUser == 2)
                    {
                        Console.WriteLine("Введите 1 для просмотра изученных слов и 2 для просмотра неизученных слов");
                        var answerNumberViewer = program.GetConsoleInt();
                        if (answerNumberViewer == 1)
                        {
                            var examinedWords = program.ViewExaminedWords();
                            if (examinedWords.Count > 0)
                            {
                                foreach (var examinedWordsTemp in examinedWords)
                                {

                                    Console.WriteLine(examinedWordsTemp.WordInEnglish);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Таких слов нет");
                            }
                        }
                        else
                        {
                            var unexaminedWords = program.ViewUnexaminedWords();
                            if (unexaminedWords.Count > 0)
                            {
                                foreach (var unexaminedWordsTemp in unexaminedWords)
                                {
                                    Console.WriteLine(unexaminedWordsTemp.WordInEnglish);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Таких слов нет");
                            }
                        }
                    }
                    if( answerNumberUser == 0)
                    {
                        break;
                    }
                }
            }
        }

        public int GetConsoleInt()
        {
            return int.Parse(Console.ReadLine());
        }

        public string GetConsoleString()
        {
            return Console.ReadLine().ToString();
        }

        public User Register(string nickname)
        {
            Guid userId = UserService.RegisterUser(nickname);
            return UserRepository.LoadUser(UserService.RegisterUser(nickname));
        }

        public User LogIn(string nickname)
        {
            return UserService.LogIn(nickname);
        }

        public void SetUp(User user)
        {
            ExamineService = new ExamineService(user,new InMemoryWordRepository());
            ViewerService = new ViewerService(user,new InMemoryWordRepository());
        }

        public IList<Word> GetTwoWords()
        {
            return ExamineService.GetTwoWords();
        }

        public bool Check(bool answer, IList<Word> twoWords)
        {
            return ExamineService.Check(answer, twoWords);
        }

        public IList<Word> ViewExaminedWords()
        {
            return ViewerService.ViewExaminedWords();
        }

        public IList<Word> ViewUnexaminedWords()
        {
            return ViewerService.ViewUnexaminedWords();
        }

    }
}
