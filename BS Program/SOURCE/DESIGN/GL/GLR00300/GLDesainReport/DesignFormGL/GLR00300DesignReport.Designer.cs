using FastReport;
using System.Collections;

namespace DesignFormGL
{
    partial class GLR00300DesignReport : Form
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            buttonFormatA_D = new Button();
            buttonFormatE_H = new Button();
            PMR0150Summary = new Button();
            PMR0150Detail = new Button();
            SuspendLayout();
            // 
            // buttonFormatA_D
            // 
            buttonFormatA_D.Location = new Point(14, 75);
            buttonFormatA_D.Margin = new Padding(3, 4, 3, 4);
            buttonFormatA_D.Name = "buttonFormatA_D";
            buttonFormatA_D.Size = new Size(202, 31);
            buttonFormatA_D.TabIndex = 3;
            buttonFormatA_D.Text = "Fomart A_D Report";
            buttonFormatA_D.UseVisualStyleBackColor = true;
            buttonFormatA_D.Click += ButtonFormatA_D_Click;
            // 
            // buttonFormatE_H
            // 
            buttonFormatE_H.Location = new Point(14, 132);
            buttonFormatE_H.Margin = new Padding(3, 4, 3, 4);
            buttonFormatE_H.Name = "buttonFormatE_H";
            buttonFormatE_H.Size = new Size(202, 31);
            buttonFormatE_H.TabIndex = 3;
            buttonFormatE_H.Text = "Fomart E_H Report";
            buttonFormatE_H.UseVisualStyleBackColor = true;
            buttonFormatE_H.Click += ButtonFormatE_H_Click;
            // 
            // PMR0150Summary
            // 
            PMR0150Summary.Location = new Point(239, 132);
            PMR0150Summary.Margin = new Padding(3, 4, 3, 4);
            PMR0150Summary.Name = "PMR0150Summary";
            PMR0150Summary.Size = new Size(202, 31);
            PMR0150Summary.TabIndex = 4;
            PMR0150Summary.Text = "PMR0150Summary";
            PMR0150Summary.UseVisualStyleBackColor = true;
            PMR0150Summary.Click += PMR0150Summary_Click;
            // 
            // button1
            // 
            PMR0150Detail.Location = new Point(239, 75);
            PMR0150Detail.Margin = new Padding(3, 4, 3, 4);
            PMR0150Detail.Name = "PMR0150Detail";
            PMR0150Detail.Size = new Size(202, 31);
            PMR0150Detail.TabIndex = 5;
            PMR0150Detail.Text = "PMR0150Detail";
            PMR0150Detail.UseVisualStyleBackColor = true;
            PMR0150Detail.Click += PMR00150Detail_Click;
            // 
            // GLR00300DesignReport
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(buttonFormatA_D);
            Controls.Add(buttonFormatE_H);
            Controls.Add(PMR0150Summary);
            Controls.Add(PMR0150Detail);
            Margin = new Padding(3, 4, 3, 4);
            Name = "GLR00300DesignReport";
            Text = "DesignForm";
            Load += GLDesainReport_Load;
            ResumeLayout(false);
        }

        private Button buttonFormatA_D;
        private Button buttonFormatE_H;
        private Button PMR0150Summary;
        private Button PMR0150Detail;
    }
}