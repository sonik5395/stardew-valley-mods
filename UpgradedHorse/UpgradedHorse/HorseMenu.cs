﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI;

namespace UpgradedHorseMod
{
    public class HorseMenu : IClickableMenu
    {
        public new static int width = Game1.tileSize * 6;
        public new static int height = Game1.tileSize * 8;
        private string hoverText = "";
        public const int region_okButton = 101;
        public const int region_love = 102;
        public const int region_sellButton = 103;
        public const int region_fullnessHover = 107;
        public const int region_happinessHover = 108;
        public const int region_loveHover = 109;
        public const int region_textBoxCC = 110;

        private UpgradedHorse horse;
        private TextBox textBox;
        public ClickableTextureComponent okButton;
        public ClickableTextureComponent love;
        public ClickableComponent fullnessHover;
        public ClickableComponent happinessHover;
        public ClickableComponent loveHover;
        public ClickableComponent textBoxCC;
        private double fullnessLevel;
        private double happinessLevel;
        private double loveLevel;
        private bool movinghorse;

        public HorseMenu(UpgradedHorse horse)
          : base(Game1.viewport.Width / 2 - HorseMenu.width / 2, Game1.viewport.Height / 2 - HorseMenu.height / 2, HorseMenu.width, HorseMenu.height, false)
        {
            Game1.player.Halt();
            HorseMenu.width = Game1.tileSize * 6;
            HorseMenu.height = Game1.tileSize * 8;

            this.horse = horse;
            this.textBox = new TextBox((Texture2D)null, (Texture2D)null, Game1.dialogueFont, Game1.textColor);
            this.textBox.X = Game1.viewport.Width / 2 - Game1.tileSize * 2 - 12;
            this.textBox.Y = this.yPositionOnScreen - 4 + Game1.tileSize * 2;
            this.textBox.Width = Game1.tileSize * 4;
            this.textBox.Height = Game1.tileSize * 3;

            this.textBoxCC = new ClickableComponent(new Microsoft.Xna.Framework.Rectangle(this.textBox.X, this.textBox.Y, this.textBox.Width, Game1.tileSize), "")
            {
                myID = 110,
                downNeighborID = 104
            };
            this.textBox.Text = "Test123333333";

            this.textBox.Selected = false;

            ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + HorseMenu.width + 4, this.yPositionOnScreen + HorseMenu.height - Game1.tileSize - IClickableMenu.borderWidth, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);
            int num1 = 101;
            textureComponent1.myID = num1;
            int num2 = 103;
            textureComponent1.upNeighborID = num2;
            this.okButton = textureComponent1;



            ClickableTextureComponent textureComponent5 = new ClickableTextureComponent((Math.Round((double)horse.friendshipTowardFarmer, 0) / 10.0).ToString() + "<", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize / 2 + 16, this.yPositionOnScreen - Game1.tileSize / 2 + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 4 - Game1.tileSize / 2, HorseMenu.width - Game1.tileSize * 2, Game1.tileSize), (string)null, "Friendship", Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(172, 512, 16, 16), 4f, false);
            int num10 = 102;
            textureComponent5.myID = num10;
            this.love = textureComponent5;
            this.loveHover = new ClickableComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 3 - Game1.tileSize / 2, HorseMenu.width, Game1.tileSize), "Friendship")
            {
                myID = 109
            };
            this.fullnessLevel = (double)horse.fullness / (double)byte.MaxValue;

            this.happinessLevel = (double)horse.happiness / (double)byte.MaxValue;
            this.loveLevel = (double)horse.friendshipTowardFarmer / 1000.0;
            if (!Game1.options.SnappyMenus)
                return;
            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            this.currentlySnappedComponent = this.getComponentWithID(101);
            this.snapCursorToCurrentSnappedComponent();
        }

        public void textBoxEnter(TextBox sender)
        {
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.globalFade)
                return;
            if (((IEnumerable<InputButton>)Game1.options.menuButton).Contains<InputButton>(new InputButton(key)) && (this.textBox == null || !this.textBox.Selected))
            {
                Game1.playSound("smallSelect");
                if (this.readyToClose())
                {
                    Game1.exitActiveMenu();
                    if (this.textBox.Text.Length <= 0)
                        return;
                    this.horse.displayName = this.textBox.Text;
                    this.horse.name.Set(this.textBox.Text);
                }
                else
                {
                    if (!this.movinghorse)
                        return;
                    Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.prepareForReturnFromPlacement), 0.02f);
                }
            }
            else
            {
                if (!Game1.options.SnappyMenus || ((IEnumerable<InputButton>)Game1.options.menuButton).Contains<InputButton>(new InputButton(key)) && this.textBox != null && this.textBox.Selected)
                    return;
                base.receiveKeyPress(key);
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (!this.movinghorse)
                return;
            int num1 = Game1.getOldMouseX() + Game1.viewport.X;
            int num2 = Game1.getOldMouseY() + Game1.viewport.Y;
            if (num1 - Game1.viewport.X < Game1.tileSize)
                Game1.panScreen(-8, 0);
            else if (num1 - (Game1.viewport.X + Game1.viewport.Width) >= -Game1.tileSize)
                Game1.panScreen(8, 0);
            if (num2 - Game1.viewport.Y < Game1.tileSize)
                Game1.panScreen(0, -8);
            else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -Game1.tileSize)
                Game1.panScreen(0, 8);
            //foreach (Keys pressedKey in Game1.oldKBState.GetPressedKeys())
            //    this.receiveKeyPress(pressedKey);
        }

        public void finishedPlacinghorse()
        {
            Game1.exitActiveMenu();
            Game1.currentLocation = Game1.player.currentLocation;
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear((Game1.afterFadeFunction)null, 0.02f);
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.displayFarmer = true;
            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:horseQuery_Moving_HomeChanged"), Color.LimeGreen, 3500f));
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.globalFade)
                return;

        }

        public override bool overrideSnappyMenuCursorMovementBan()
        {
            return this.movinghorse;
        }

        public void prepareForhorsePlacement()
        {
            this.movinghorse = true;
            Game1.currentLocation = Game1.getLocationFromName("Farm");
            Game1.globalFadeToClear((Game1.afterFadeFunction)null, 0.02f);
            this.okButton.bounds.X = Game1.viewport.Width - Game1.tileSize * 2;
            this.okButton.bounds.Y = Game1.viewport.Height - Game1.tileSize * 2;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = new Location(49 * Game1.tileSize, 5 * Game1.tileSize);
            Game1.panScreen(0, 0);
            Game1.currentLocation.resetForPlayerEntry();
            Game1.displayFarmer = false;
        }

        public void prepareForReturnFromPlacement()
        {
            Game1.currentLocation = Game1.player.currentLocation;
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear((Game1.afterFadeFunction)null, 0.02f);
            this.okButton.bounds.X = this.xPositionOnScreen + HorseMenu.width + 4;
            this.okButton.bounds.Y = this.yPositionOnScreen + HorseMenu.height - Game1.tileSize - IClickableMenu.borderWidth;
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.displayFarmer = true;
            this.movinghorse = false;
        }

        public override bool readyToClose()
        {
            this.textBox.Selected = false;
            if (base.readyToClose() && !this.movinghorse)
                return !Game1.globalFade;
            return false;
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (Game1.globalFade)
                return;
            if (this.readyToClose())
            {
                Game1.exitActiveMenu();
                if (this.textBox.Text.Length > 0)
                {
                    this.horse.displayName = this.textBox.Text;
                    this.horse.name.Set(this.textBox.Text);
                }
                Game1.playSound("smallSelect");
            }
            else
            {
                if (!this.movinghorse)
                    return;
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.prepareForReturnFromPlacement), 0.02f);
            }
        }

        public override void performHoverAction(int x, int y)
        {
            this.hoverText = "";
            if (this.movinghorse)
            {
                //
            }
            if (this.okButton != null)
            {
                if (this.okButton.containsPoint(x, y))
                    this.okButton.scale = Math.Min(1.1f, this.okButton.scale + 0.05f);
                else
                    this.okButton.scale = Math.Max(1f, this.okButton.scale - 0.05f);
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (!this.movinghorse && !Game1.globalFade)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen + Game1.tileSize * 2, HorseMenu.width, HorseMenu.height - Game1.tileSize * 2, false, true, (string)null, false);

                string text1 = this.horse.displayName;

                /* Drawing Love Level */
                Utility.drawTextWithShadow(b, text1, Game1.smallFont, new Vector2((float)(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize / 2), (float)(this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4 + Game1.tileSize * 2)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                int num2 = 0;
                int num3 = this.loveLevel * 1000.0 % 200.0 >= 100.0 ? (int)(this.loveLevel * 1000.0 / 200.0) : -100;
                for (int index = 0; index < 5; ++index)
                {
                    b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3 / 2 + 8 * Game1.pixelZoom * index), (float)(num2 + this.yPositionOnScreen - Game1.tileSize / 2 + Game1.tileSize * 5)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(211 + (this.loveLevel * 1000.0 <= (double)((index + 1) * 195) ? 7 : 0), 428, 7, 6)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.89f);
                    if (num3 == index)
                        b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3 / 2 + 8 * Game1.pixelZoom * index), (float)(num2 + this.yPositionOnScreen - Game1.tileSize / 2 + Game1.tileSize * 5)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(211, 428, 4, 6)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.891f);
                }

                // Draw Fullness Level
                int num4 = 0;
                int num5 = this.fullnessLevel * 1000.0 % 200.0 >= 100.0 ? (int)(this.loveLevel * 1000.0 / 200.0) : -100;
                for (int index = 0; index < 5; ++index)
                {
                    b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3 / 2 + 8 * Game1.pixelZoom * index), (float)(num4 + this.yPositionOnScreen - Game1.tileSize / 2 + Game1.tileSize * 6)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(211 + (this.loveLevel * 1000.0 <= (double)((index + 1) * 195) ? 7 : 0), 428, 7, 6)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.89f);
                    if (num5 == index)
                        b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3 / 2 + 8 * Game1.pixelZoom * index), (float)(num4 + this.yPositionOnScreen - Game1.tileSize / 2 + Game1.tileSize * 6)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(211, 428, 4, 6)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.891f);
                }


                Utility.drawTextWithShadow(b, Game1.parseText(this.horse.getMoodMessage(), Game1.smallFont, HorseMenu.width - IClickableMenu.spaceToClearSideBorder * 2 - Game1.tileSize), Game1.smallFont, new Vector2((float)(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize / 2), (float)(num2 + this.yPositionOnScreen + Game1.tileSize * 7 - Game1.tileSize + Game1.pixelZoom)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                this.okButton.draw(b);

                if (this.hoverText != null && this.hoverText.Length > 0)
                    IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
            }
            else if (!Game1.globalFade)
            {
                string text = "test";
                Game1.drawDialogueBox(Game1.tileSize / 2, -Game1.tileSize, (int)Game1.dialogueFont.MeasureString(text).X + IClickableMenu.borderWidth * 2 + Game1.tileSize / 4, Game1.tileSize * 2 + IClickableMenu.borderWidth * 2, false, true, (string)null, false);
                b.DrawString(Game1.dialogueFont, text, new Vector2((float)(Game1.tileSize / 2 + IClickableMenu.spaceToClearSideBorder * 2 + 8), (float)(Game1.tileSize / 2 + Game1.pixelZoom * 3)), Game1.textColor);
                this.okButton.draw(b);
            }
            this.drawMouse(b);
        }
    }
}