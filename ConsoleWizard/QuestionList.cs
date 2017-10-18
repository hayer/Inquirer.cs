﻿using System;
using System.Linq;
using ConsoleWizard.Components;

namespace ConsoleWizard
{
    public class QuestionList<TResult> : QuestionListBase<TResult> where TResult : IComparable
    {
        internal QuestionList(string question) : base(question)
        {
        }

        public Func<int, TResult> ParseFn { get; set; } = v => { return default(TResult); };

        public Func<int, bool> ValidatationFn { get; set; } = v => { return true; };

        public QuestionList<TResult> ConvertToString(Func<TResult, string> fn)
        {
            ConvertToStringFn = fn;
            return this;
        }

        public QuestionList<TResult> Parse(Func<int, TResult> fn)
        {
            ParseFn = fn;
            return this;
        }

        public QuestionList<TResult> Validation(Func<int, bool> fn)
        {
            ValidatationFn = fn;
            return this;
        }

        public QuestionList<TResult> WithConfirmation()
        {
            HasConfirmation = true;
            return this;
        }

        public QuestionList<TResult> WithDefaultValue<T>(T defaultValue) where T : IComparable
        {
            if (Choices.Where(x => x.CompareTo(defaultValue) == 0).Any())
            {
                var index = Choices.Select((v, i) => new { Value = v, Index = i }).First(x => x.Value.CompareTo(defaultValue) == 0).Index;
                Choices.Insert(0, Choices[index]);
                Choices.RemoveAt(index + 1);

                DefaultValue = Choices[0];
                HasDefaultValue = true;
            }
            else
            {
                throw new Exception("No default values in choices");
            }

            return this;
        }

        internal override TResult Prompt()
        {
            bool tryAgain = true;
            TResult answer = DefaultValue;

            while (tryAgain)
            {
                DisplayQuestion();

                Console.WriteLine();
                Console.WriteLine();

                for (int i = 0; i < Choices.Count; i++)
                {
                    ConsoleHelper.WriteLine("  " + DisplayChoice(i));
                }

                Console.CursorVisible = false;

                int boundryTop = Console.CursorTop - Choices.Count;
                int boundryBottom = boundryTop + Choices.Count - 1;

                ConsoleHelper.PositionWrite("→", 0, boundryTop);

                bool move = true;
                while (move)
                {
                    int y = Console.CursorTop;

                    bool isCanceled = false;
                    var key = ConsoleHelper.ReadKey(out isCanceled);
                    if (isCanceled)
                    {
                        IsCanceled = isCanceled;
                        return default(TResult);
                    }

                    Console.SetCursorPosition(0, y);
                    ConsoleHelper.Write("  " + DisplayChoice(y - boundryTop));
                    Console.SetCursorPosition(0, y);

                    switch (key)
                    {
                        case (ConsoleKey.UpArrow):
                            {
                                if (y > boundryTop)
                                {
                                    y -= 1;
                                }

                                break;
                            }

                        case (ConsoleKey.DownArrow):
                            {
                                if (y < boundryBottom)
                                {
                                    y += 1;
                                }

                                break;
                            }

                        case (ConsoleKey.Enter):
                            {
                                Console.CursorVisible = true;
                                answer = Choices[Console.CursorTop - boundryTop];
                                move = false;
                                break;
                            }
                    }

                    ConsoleHelper.PositionWrite("  " + DisplayChoice(y - boundryTop), 0, y, ConsoleColor.DarkYellow);
                    ConsoleHelper.PositionWrite("→", 0, y);
                    Console.SetCursorPosition(0, y);
                }

                tryAgain = Confirm(ConvertToStringFn(answer));
            }

            Console.WriteLine();
            return answer;
        }

        protected string DisplayChoice(int index)
        {
            return $"{Choices[index]}";
        }
    }
}
