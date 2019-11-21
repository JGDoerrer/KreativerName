using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KreativerName.UI;
using KreativerName.UI.Constraints;
using OpenTK;
using OpenTK.Input;

namespace KreativerName.Scenes
{
    public class Statistics : Scene
    {
        public Statistics()
        {
            InitUI();
        }

        private void InitUI()
        {
            ui = new UI.UI();
            ui.Input = new Input(Scenes.Window);

            void AddText(string s, string value, int y)
            {
                TextBlock text1 = new TextBlock(s, 3);
                text1.Color = Color.White;
                text1.SetConstraints(new CenterConstraint(-text1.TextWidth / 2f), new PixelConstraint(y), new PixelConstraint((int)text1.TextWidth), new PixelConstraint((int)text1.TextHeight));
                ui.Add(text1);

                TextBlock text2 = new TextBlock(value, 3);
                text2.Color = Color.White;
                text2.SetConstraints(new CenterConstraint(text2.TextWidth / 2f), new PixelConstraint(y), new PixelConstraint((int)text2.TextWidth), new PixelConstraint((int)text2.TextHeight));
                ui.Add(text2);
            }

            TextBlock title = new TextBlock("Statistik", 4);
            title.Color = Color.White;
            title.SetConstraints(new CenterConstraint(), new PixelConstraint(50), new PixelConstraint((int)title.TextWidth), new PixelConstraint((int)title.TextHeight));
            ui.Add(title);

            AddText("Zuege: ", Stats.Current.TotalMoves.ToString(), 100);
            AddText("Level geschafft: ", Stats.Current.LevelsCompleted.ToString(), 130);
            AddText("Level perfekt geschafft: ", Stats.Current.LevelsCompletedPerfect.ToString(), 160);
            AddText("Erstes Spiel: ",Stats.Current.FirstStart.ToString("dd.mm.yy"), 190);
            AddText("Spielzeit: ", Stats.Current.TimePlaying.ToString("hh\\:mm\\:ss"), 220);
            AddText("Tode: ", Stats.Current.Deaths.ToString(), 250);
        }

        UI.UI ui;

        public override void Update()
        {
            if (Scenes.Input.KeyPress(Key.Escape))
                Scenes.LoadScene(new Transition(new MainMenu(), 10));
        }

        public override void UpdateUI(Vector2 windowSize)
        {
            ui.Update(windowSize);
        }

        public override void Render(Vector2 windowSize)
        {
            ui.Render(windowSize);
        }
    }
}
