﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace InquirerCS
{
    public abstract class QuestionMultipleListBase<TList, T> : QuestionBase<TList> where TList : List<T>
    {
        private TList _choices;

        internal QuestionMultipleListBase(string message) : base(message)
        {
        }

        internal Func<T, string> ConvertToStringFn { get; set; } = value => { return value.ToString(); };

        internal TList Choices
        {
            get
            {
                return _choices;
            }

            set
            {
                _choices = value;
                Selected = new bool[value.Count];
            }
        }

        internal bool[] Selected { get; private set; }

        public void DisplayQuestion()
        {
            Console.Clear();
            ConsoleHelper.Write("[?] ", ConsoleColor.Yellow);
            var question = $"{Message} : ";
            if (HasDefaultValue)
            {
                question += $"[{string.Join(",", DefaultValue.Select(x => ConvertToStringFn(x)))}] ";
            }

            ConsoleHelper.Write(question);
        }
    }
}