using System;
using System.Drawing;
using System.Windows.Forms;

namespace Backdrop
{
    public partial class BackdropForm : Form
    {
        public BackdropForm()
        {
            InitializeComponent();

            // 1. 全画面表示設定
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            
            // 2. 背景色を白に設定
            this.BackColor = Color.White;

            // 3. キーイベントをフォームが受け取るように設定
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form_KeyDown);
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            // ESCキーが押されたら終了
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            // スペースキーで背景色切り替え
            else if (e.KeyCode == Keys.Space)
            {
                if (this.BackColor == Color.White)
                {
                    // ダークモード (#303030)
                    this.BackColor = Color.FromArgb(0x30, 0x30, 0x30);
                }
                else
                {
                    // 白に戻す
                    this.BackColor = Color.White;
                }
            }
        }
    }
}

