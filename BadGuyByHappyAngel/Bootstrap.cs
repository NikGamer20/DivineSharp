using Divine;
using SharpDX;
using Divine.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using Divine.Menu.Items;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Divine.Menu.EventArgs;

namespace BadGuyByHappyAngel
{
    public class Bootstrap : Bootstrapper
    {
        private MenuSwitcher EnableSwitcher;

        private MenuSelector LangSelector;

        private MenuSwitcher AutoPauseSwitcher;

        private Random Random;

        private readonly string[] EngText =
        {
            "You only need to play supports",
            "LOX",
            "useless",
            "Remove the game mediocre",
            "PRIVET LOX",
            "You lose",
            "Solo one on one",
            "???","1000 - 7?"
        };

        private readonly string[] RusText =
        {
            "???",
            "1000 - 7?",
            "Удали игру мой друг ИВАН и то лучше играет",
            "ПЕТУХ",
            "Слабый",
            "Я лучше тебя ^)",
            "Смотри на мою игру лучше",
            "НЮХАЙ БЕБРУ",
            "Раз на раз цука",
            "Единственный",
            "Купи мозг плиз"
        };

        protected override void OnActivate()
        {
            var rootmenu = MenuManager.CreateRootMenu("Bad guy by HappyAngel");
            EnableSwitcher = rootmenu.CreateSwitcher("Enable", false);
            AutoPauseSwitcher = rootmenu.CreateSwitcher("Auto pause", false);
            LangSelector = rootmenu.CreateSelector("Lang", new[] { "Eng", "Rus" });

            EnableSwitcher.ValueChanged += OnEnableValueChanged;
        }

        private void OnEnableValueChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (e.Value)
            {
                Random = new Random();
                GameManager.GameEvent += OnGameEvent;
            }
            else
            {
                GameManager.GameEvent -= OnGameEvent;
            }
        }

        private async void OnGameEvent(GameEventEventArgs e)
        {
            var gameEvent = e.GameEvent;

            if (gameEvent.Name != "entity_killed" ||
                gameEvent.GetInt32("entindex_attacker") != EntityManager.LocalHero.Index ||
                EntityManager.GetEntityByIndex(gameEvent.GetInt32("entindex_killed")) is not Hero hero ||
                hero.IsIllusion)
            {
                return;
            }

            await Task.Delay(500);

            if (AutoPauseSwitcher)
            {
                GameManager.ExecuteCommand("dota_pause");
            }

            var text = LangSelector == "Eng" ? EngText : RusText;
            var index = Random.Next(0, text.Length);

            GameManager.ExecuteCommand($"say {text[index]}");
        }
    }
}