using System;
using System.Windows.Forms;

namespace WangPluginPkm.GUI
{
    partial class ShinyMakerUI
    {
        private Button ShinySID16_BTN;
        private Button RandomStar_BTN;
        private Button ForceSquare_BTN;
        private ComboBox XorBox;
        private Button Xor_BTN;
        private Button ForceStar;
        private Label XorValue_Label;
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShinyMakerUI));
            ShinySID16_BTN = new Button();
            RandomStar_BTN = new Button();
            ForceSquare_BTN = new Button();
            XorBox = new ComboBox();
            Xor_BTN = new Button();
            ForceStar = new Button();
            XorValue_Label = new Label();
            RangeBox = new ComboBox();
            GEN1GEN2 = new Button();
            SuspendLayout();
            // 
            // ShinySID16_BTN
            // 
            ShinySID16_BTN.Font = new System.Drawing.Font("黑体", 9F);
            ShinySID16_BTN.Location = new System.Drawing.Point(130, 54);
            ShinySID16_BTN.Name = "ShinySID16_BTN";
            ShinySID16_BTN.Size = new System.Drawing.Size(120, 30);
            ShinySID16_BTN.TabIndex = 0;
            ShinySID16_BTN.Text = "随机SID16闪光";
            ShinySID16_BTN.UseVisualStyleBackColor = true;
            ShinySID16_BTN.Click += ShinySID16_BTN_Click;
            // 
            // RandomStar_BTN
            // 
            RandomStar_BTN.Font = new System.Drawing.Font("黑体", 9F);
            RandomStar_BTN.Location = new System.Drawing.Point(130, 9);
            RandomStar_BTN.Name = "RandomStar_BTN";
            RandomStar_BTN.Size = new System.Drawing.Size(120, 30);
            RandomStar_BTN.TabIndex = 1;
            RandomStar_BTN.Text = "一键闪光";
            RandomStar_BTN.UseVisualStyleBackColor = true;
            RandomStar_BTN.Click += Shiny_BTN_Click;
            // 
            // ForceSquare_BTN
            // 
            ForceSquare_BTN.Font = new System.Drawing.Font("黑体", 9F);
            ForceSquare_BTN.Location = new System.Drawing.Point(400, 9);
            ForceSquare_BTN.Name = "ForceSquare_BTN";
            ForceSquare_BTN.Size = new System.Drawing.Size(120, 30);
            ForceSquare_BTN.TabIndex = 2;
            ForceSquare_BTN.Text = "强制方块";
            ForceSquare_BTN.UseVisualStyleBackColor = true;
            ForceSquare_BTN.Click += ForceSquare_BTN_Click;
            // 
            // XorBox
            // 
            XorBox.FormattingEnabled = true;
            XorBox.Location = new System.Drawing.Point(312, 57);
            XorBox.Name = "XorBox";
            XorBox.Size = new System.Drawing.Size(73, 25);
            XorBox.TabIndex = 3;
            // 
            // Xor_BTN
            // 
            Xor_BTN.Font = new System.Drawing.Font("黑体", 9F);
            Xor_BTN.Location = new System.Drawing.Point(400, 54);
            Xor_BTN.Name = "Xor_BTN";
            Xor_BTN.Size = new System.Drawing.Size(120, 30);
            Xor_BTN.TabIndex = 4;
            Xor_BTN.Text = "指定Xor闪光";
            Xor_BTN.UseVisualStyleBackColor = true;
            Xor_BTN.Click += Xor_BTN_Click;
            // 
            // ForceStar
            // 
            ForceStar.Font = new System.Drawing.Font("黑体", 9F);
            ForceStar.Location = new System.Drawing.Point(265, 9);
            ForceStar.Name = "ForceStar";
            ForceStar.Size = new System.Drawing.Size(120, 30);
            ForceStar.TabIndex = 5;
            ForceStar.Text = "强制星星";
            ForceStar.UseVisualStyleBackColor = true;
            ForceStar.Click += ForceStar_Click;
            // 
            // XorValue_Label
            // 
            XorValue_Label.AutoSize = true;
            XorValue_Label.Font = new System.Drawing.Font("Arial", 10.2F);
            XorValue_Label.Location = new System.Drawing.Point(261, 61);
            XorValue_Label.Name = "XorValue_Label";
            XorValue_Label.Size = new System.Drawing.Size(36, 16);
            XorValue_Label.TabIndex = 6;
            XorValue_Label.Text = "Xor=";
            // 
            // RangeBox
            // 
            RangeBox.FormattingEnabled = true;
            RangeBox.Location = new System.Drawing.Point(12, 12);
            RangeBox.Name = "RangeBox";
            RangeBox.Size = new System.Drawing.Size(112, 25);
            RangeBox.TabIndex = 7;
            // 
            // GEN1GEN2
            // 
            GEN1GEN2.Font = new System.Drawing.Font("黑体", 9F);
            GEN1GEN2.Location = new System.Drawing.Point(12, 54);
            GEN1GEN2.Name = "GEN1GEN2";
            GEN1GEN2.Size = new System.Drawing.Size(112, 30);
            GEN1GEN2.TabIndex = 8;
            GEN1GEN2.Text = "初代二代闪光";
            GEN1GEN2.UseVisualStyleBackColor = true;
            GEN1GEN2.Click += GEN1GEN2_SHING_Click;
            // 
            // ShinyMakerUI
            // 
            AllowDrop = true;
            ClientSize = new System.Drawing.Size(549, 97);
            Controls.Add(GEN1GEN2);
            Controls.Add(RangeBox);
            Controls.Add(XorValue_Label);
            Controls.Add(ForceStar);
            Controls.Add(Xor_BTN);
            Controls.Add(XorBox);
            Controls.Add(ForceSquare_BTN);
            Controls.Add(RandomStar_BTN);
            Controls.Add(ShinySID16_BTN);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "ShinyMakerUI";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Super Wang";
            ResumeLayout(false);
            PerformLayout();
        }

        private ComboBox RangeBox;
        private Button GEN1GEN2;
    }
 }
