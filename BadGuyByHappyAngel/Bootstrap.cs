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
using Divine.SDK.Extensions;
using Divine.SDK.Helpers;

namespace BadGuyByHappyAngel
{
    public class Bootstrap : Bootstrapper
    {
        private MenuSwitcher EnableSwitcher;

        private MenuSwitcher AutoPauseOnKillHeroSwitcher;

        private MenuSwitcher AutoChatSwitcher;

        private MenuSelector LangSelector;

        private MenuSwitcher AutoFeedSwitcher;

        private MenuSwitcher AutoTauntOnKillHeroSwitcher;

        private MenuSwitcher AutoTauntSwitcher;
        
        private MenuSwitcher AutoLolOnKillHeroSwitcher;

        private MenuSwitcher AutoLolSwitcher;

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
            "???",
            "1000 - 7?"
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
            "Помойка",
            "Я ТОП 1 ",
            "Твой рейт максимум единица чел...",
            "Чел ты..."
        };

        protected override void OnActivate()
        {
            var rootmenu = MenuManager.CreateRootMenu("Bad guy by HappyAngel");
            EnableSwitcher = rootmenu.CreateSwitcher("Enable", false);
            AutoPauseOnKillHeroSwitcher = rootmenu.CreateSwitcher("Auto pause on kill hero", false);
            AutoChatSwitcher = rootmenu.CreateSwitcher("Auto chat on kill hero", false);
            LangSelector = rootmenu.CreateSelector("Lang", new[] { "Eng", "Rus" });
            AutoFeedSwitcher = rootmenu.CreateSwitcher("Auto feed", false);
            AutoTauntOnKillHeroSwitcher = rootmenu.CreateSwitcher("Auto taunt on kill hero", false);
            AutoTauntSwitcher = rootmenu.CreateSwitcher("Auto taunt", false);
            AutoLolOnKillHeroSwitcher = rootmenu.CreateSwitcher("Auto lol on kill hero", false);
            AutoLolSwitcher = rootmenu.CreateSwitcher("Auto lol", false);

            EnableSwitcher.ValueChanged += OnEnableValueChanged;
        }

        private void OnEnableValueChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (e.Value)
            {
                Random = new Random();
                GameManager.GameEvent += OnGameEvent;
                UpdateManager.CreateIngameUpdate(1000, OnUpdate);
            }
            else
            {
                GameManager.GameEvent -= OnGameEvent;
                UpdateManager.DestroyIngameUpdate(OnUpdate);
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

            if (AutoPauseOnKillHeroSwitcher)
            {
                GameManager.ExecuteCommand("dota_pause");
            }

            if (AutoTauntOnKillHeroSwitcher && !MultiSleeper<string>.Sleeping("AutoTaunt"))
            {
                GameManager.ExecuteCommand("use_item_client current_hero taunt");
                MultiSleeper<string>.Sleep("AutoTaunt", 6000);
            }

            if (AutoLolOnKillHeroSwitcher && !MultiSleeper<string>.Sleeping("AutoLol"))
            {
                GameManager.ExecuteCommand("say lol");
                MultiSleeper<string>.Sleep("AutoLol", 15000);

                await Task.Delay(300);
            }

            if (AutoChatSwitcher)
            {
                var text = LangSelector == "Eng" ? EngText : RusText;
                var index = Random.Next(0, text.Length);

                GameManager.ExecuteCommand($"say {text[index]}");
            }
        }

        private void OnUpdate()
        {
            var localHero = EntityManager.LocalHero;
            if (localHero == null || !localHero.IsValid || GameManager.IsPaused)
            {
                return;
            }

            if (AutoFeedSwitcher && !MultiSleeper<string>.Sleeping("AutoFeed"))
            {
                var fort = EntityManager.GetEntities<Fort>().FirstOrDefault(x => !x.IsAlly(localHero));
                if (fort != null)
                {
                    localHero.Attack(fort.Position);
                }

                MultiSleeper<string>.Sleep("AutoFeed", 5000);
            }

            if (AutoTauntSwitcher && !MultiSleeper<string>.Sleeping("AutoTaunt"))
            {
                GameManager.ExecuteCommand("use_item_client current_hero taunt");
                MultiSleeper<string>.Sleep("AutoTaunt", 7500);
            }

            if (AutoLolSwitcher && !MultiSleeper<string>.Sleeping("AutoLol"))
            {
                GameManager.ExecuteCommand("say lol");
                MultiSleeper<string>.Sleep("AutoLol", 15000);
            }
        }
    }
}
