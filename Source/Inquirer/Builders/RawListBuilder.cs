﻿using System;
using System.Collections.Generic;
using System.Linq;
using InquirerCS.Components;
using InquirerCS.Interfaces;
using InquirerCS.Questions;
using InquirerCS.Traits;

namespace InquirerCS.Builders
{
    public class RawListBuilder<TResult> : InputBuilder<RawList<TResult>, string, TResult>, IRenderChoicesTrait<TResult> where TResult : IComparable
    {
        public RawListBuilder(string message, IEnumerable<TResult> choices, IConsole console) : base(console)
        {
            Choices = choices.ToList();
            Console = console;

            this.RenderQuestion(message, this, this, console);
            this.Parse(value =>
            {
                return Choices[value.To<int>() - 1];
            });

            this.RenderRawChoices(Choices, this, Console);

            InputValidators.Add(value => { return string.IsNullOrEmpty(value) == false || Default.HasDefault; }, "Empty line");
            InputValidators.Add(value => { return value.ToN<int>().HasValue; }, value => { return $"Cannot parse {value} to {typeof(TResult)}"; });
            InputValidators.Add(
            value =>
            {
                var index = value.To<int>();
                return index > 0 && index <= Choices.Count;
            },
            value =>
            {
                return $"Chosen number must be between 1 and {Choices.Count}";
            });

            this.Input(Console, value => { return char.IsNumber(value); });
        }

        public List<TResult> Choices { get; set; }

        public IRenderChoices<TResult> RenderChoices { get; set; }

        public override RawList<TResult> Build()
        {
            return new RawList<TResult>(Choices, Confirm, RenderQuestion, Input, Parse, RenderChoices, ResultValidators, InputValidators, DisplayError, OnKey, Console);
        }

        public PagedRawListBuilder<TResult> Page(int pageSize)
        {
            return new PagedRawListBuilder<TResult>(this, pageSize);
        }

        public new RawListBuilder<TResult> WithConfirmation()
        {
            this.Confirm(this, Console);
            return this;
        }

        public new RawListBuilder<TResult> WithConvertToString(Func<TResult, string> fn)
        {
            this.ConvertToString(fn);
            return this;
        }

        public new RawListBuilder<TResult> WithDefaultValue(TResult defaultValue)
        {
            Default = new DefaultListValueComponent<TResult>(Choices, defaultValue);
            return this;
        }

        public new RawListBuilder<TResult> WithValidation(Func<TResult, bool> fn, Func<TResult, string> errorMessageFn)
        {
            ResultValidators.Add(fn, errorMessageFn);
            return this;
        }

        public new RawListBuilder<TResult> WithValidation(Func<TResult, bool> fn, string errorMessage)
        {
            ResultValidators.Add(fn, errorMessage);
            return this;
        }
    }
}