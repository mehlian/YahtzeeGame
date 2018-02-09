﻿using NSubstitute;
using NUnit.Framework;
using System;
using System.Windows.Input;
using Yahtzee.Core;

namespace Yahtzee.ViewModels.UnitTests
{
    [TestFixture]
    public class GameWindowViewModelTests
    {
        private IRandomizer _randomizer;

        [SetUp]
        public void Setup()
        {
            _randomizer = Substitute.For<IRandomizer>();
        }

        [Test]
        public void GameWindowViewModel_CanBeCreated()
        {
            GameWindowViewModel viewModel = new GameWindowViewModel( _randomizer);
        }

        [Test]
        public void RollDiceCommand_InitialState_CanBeExecuted()
        {
            GameWindowViewModel viewModel = new GameWindowViewModel(_randomizer);
            ICommand command = viewModel.RollDiceCommand;
            command.Execute(null);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void RollDiceCommand_FirstPlayerStartsTheGameWithFirstRoll_ReturnsRollResults(
            int rollResult)
        {
            GameWindowViewModel viewModel = new GameWindowViewModel(_randomizer);
            _randomizer.Roll(1, 6).Returns(rollResult);
            ICommand command = viewModel.RollDiceCommand;

            command.Execute(null);

            Assert.AreEqual(rollResult, viewModel.Die1);
        }

        [Test]
        public void Die1_PropChangedNotify_IsFired()
        {
            GameWindowViewModel viewModel = new GameWindowViewModel(_randomizer);
            bool hasFired = false;
            viewModel.PropertyChanged += (sender,args) =>
            {
                if (args.PropertyName == "Die1")
                    hasFired = true;
            };
            _randomizer.Roll(1, 6).Returns(1);
            ICommand command = viewModel.RollDiceCommand;

            command.Execute(null);

            Assert.IsTrue(hasFired);
        }
    }
}