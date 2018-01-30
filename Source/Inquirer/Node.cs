﻿using System;
using InquirerCS.Components;
using InquirerCS.Interfaces;
using InquirerCS.Traits;

namespace InquirerCS
{
    public class Node<TBuilder, TQuestion, TResult> : BaseNode where TBuilder : IWaitForInputTrait<StringOrKey>, IOnKeyTrait, IBuilder<TQuestion, TResult> where TQuestion : IQuestion<TResult>
    {
        private TBuilder _builder;

        private Action<TResult> _then;

        internal Node(TBuilder builder)
        {
            _builder = builder;
            _builder.Input.IntteruptedKeys.Add(ConsoleKey.Escape);
            _builder.OnKey = new OnEscape();
        }

        public override void Run()
        {
            if (History.GetRoot(this))
            {
                var answer = _builder.Build().Prompt();

                if (_builder.OnKey.IsInterrupted)
                {
                    History.Pop(this).Run();
                }
                else
                {
                    _then(answer);
                    BaseNode nextNode = History.Next(this);
                    if (nextNode != null)
                    {
                        nextNode.Run();
                    }
                }
            }
        }

        public void Then(Action<TResult> toBind)
        {
            _then = toBind;
        }

        public void Then(ref TResult toBind)
        {
            TResult temp = toBind;
            _then = answer => { temp = answer; };
            toBind = temp;
        }

        public void Then(TResult toBind)
        {
            TResult temp = toBind;
            _then = answer => { temp = answer; };
            toBind = temp;
        }

        private void Then(BaseNode node)
        {
            History.ScopeLevel++;
            History.Push(this);
            Run();
            History.ScopeLevel = ScopeLevel - 1;

            if (Child != null)
            {
                Child.Parent = null;
                Child = null;
                History.CurrentNode = this;
            }
        }
    }
}