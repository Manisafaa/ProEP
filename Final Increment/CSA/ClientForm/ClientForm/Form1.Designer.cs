namespace ClientForm
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabChatroom = new System.Windows.Forms.TabPage();
            this.sendButtonChat = new System.Windows.Forms.Button();
            this.backButtonChat = new System.Windows.Forms.Button();
            this.textTypingChat = new System.Windows.Forms.TextBox();
            this.listChatroom = new System.Windows.Forms.ListBox();
            this.tabPrivate = new System.Windows.Forms.TabPage();
            this.sendButtonPrivate = new System.Windows.Forms.Button();
            this.backButtonPrivate = new System.Windows.Forms.Button();
            this.textTypingPrivate = new System.Windows.Forms.TextBox();
            this.listPrivateConversation = new System.Windows.Forms.ListBox();
            this.listOfConversations = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabChatroom.SuspendLayout();
            this.tabPrivate.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabChatroom);
            this.tabControl1.Controls.Add(this.tabPrivate);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(297, 341);
            this.tabControl1.TabIndex = 0;
            // 
            // tabChatroom
            // 
            this.tabChatroom.Controls.Add(this.sendButtonChat);
            this.tabChatroom.Controls.Add(this.backButtonChat);
            this.tabChatroom.Controls.Add(this.textTypingChat);
            this.tabChatroom.Controls.Add(this.listChatroom);
            this.tabChatroom.Location = new System.Drawing.Point(4, 22);
            this.tabChatroom.Name = "tabChatroom";
            this.tabChatroom.Padding = new System.Windows.Forms.Padding(3);
            this.tabChatroom.Size = new System.Drawing.Size(289, 315);
            this.tabChatroom.TabIndex = 0;
            this.tabChatroom.Text = "Chatroom";
            this.tabChatroom.UseVisualStyleBackColor = true;
            // 
            // sendButtonChat
            // 
            this.sendButtonChat.Location = new System.Drawing.Point(147, 287);
            this.sendButtonChat.Name = "sendButtonChat";
            this.sendButtonChat.Size = new System.Drawing.Size(136, 23);
            this.sendButtonChat.TabIndex = 7;
            this.sendButtonChat.Text = "Send";
            this.sendButtonChat.UseVisualStyleBackColor = true;
            this.sendButtonChat.Click += new System.EventHandler(this.sendButtonChat_Click);
            // 
            // backButtonChat
            // 
            this.backButtonChat.Location = new System.Drawing.Point(7, 287);
            this.backButtonChat.Name = "backButtonChat";
            this.backButtonChat.Size = new System.Drawing.Size(134, 23);
            this.backButtonChat.TabIndex = 6;
            this.backButtonChat.Text = "Back";
            this.backButtonChat.UseVisualStyleBackColor = true;
            // 
            // textTypingChat
            // 
            this.textTypingChat.Location = new System.Drawing.Point(7, 263);
            this.textTypingChat.Name = "textTypingChat";
            this.textTypingChat.Size = new System.Drawing.Size(276, 20);
            this.textTypingChat.TabIndex = 5;
            this.textTypingChat.TextChanged += new System.EventHandler(this.textTypingChat_TextChanged);
            // 
            // listChatroom
            // 
            this.listChatroom.FormattingEnabled = true;
            this.listChatroom.Location = new System.Drawing.Point(6, 6);
            this.listChatroom.Name = "listChatroom";
            this.listChatroom.Size = new System.Drawing.Size(276, 251);
            this.listChatroom.TabIndex = 0;
            this.listChatroom.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listChatroom_MouseDoubleClick);
            // 
            // tabPrivate
            // 
            this.tabPrivate.Controls.Add(this.sendButtonPrivate);
            this.tabPrivate.Controls.Add(this.backButtonPrivate);
            this.tabPrivate.Controls.Add(this.textTypingPrivate);
            this.tabPrivate.Controls.Add(this.listPrivateConversation);
            this.tabPrivate.Controls.Add(this.listOfConversations);
            this.tabPrivate.Location = new System.Drawing.Point(4, 22);
            this.tabPrivate.Name = "tabPrivate";
            this.tabPrivate.Padding = new System.Windows.Forms.Padding(3);
            this.tabPrivate.Size = new System.Drawing.Size(289, 315);
            this.tabPrivate.TabIndex = 1;
            this.tabPrivate.Text = "Private Chat";
            this.tabPrivate.UseVisualStyleBackColor = true;
            this.tabPrivate.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tabPrivate_MouseDoubleClick);
            // 
            // sendButtonPrivate
            // 
            this.sendButtonPrivate.Location = new System.Drawing.Point(146, 285);
            this.sendButtonPrivate.Name = "sendButtonPrivate";
            this.sendButtonPrivate.Size = new System.Drawing.Size(136, 23);
            this.sendButtonPrivate.TabIndex = 4;
            this.sendButtonPrivate.Text = "Send";
            this.sendButtonPrivate.UseVisualStyleBackColor = true;
            this.sendButtonPrivate.Click += new System.EventHandler(this.sendButtonPrivate_Click);
            // 
            // backButtonPrivate
            // 
            this.backButtonPrivate.Location = new System.Drawing.Point(6, 285);
            this.backButtonPrivate.Name = "backButtonPrivate";
            this.backButtonPrivate.Size = new System.Drawing.Size(134, 23);
            this.backButtonPrivate.TabIndex = 3;
            this.backButtonPrivate.Text = "Back";
            this.backButtonPrivate.UseVisualStyleBackColor = true;
            this.backButtonPrivate.Click += new System.EventHandler(this.backButton_Click);
            // 
            // textTypingPrivate
            // 
            this.textTypingPrivate.Location = new System.Drawing.Point(6, 261);
            this.textTypingPrivate.Name = "textTypingPrivate";
            this.textTypingPrivate.Size = new System.Drawing.Size(276, 20);
            this.textTypingPrivate.TabIndex = 2;
            this.textTypingPrivate.TextChanged += new System.EventHandler(this.textTypingPrivate_TextChanged);
            // 
            // listPrivateConversation
            // 
            this.listPrivateConversation.FormattingEnabled = true;
            this.listPrivateConversation.Location = new System.Drawing.Point(6, 6);
            this.listPrivateConversation.Name = "listPrivateConversation";
            this.listPrivateConversation.Size = new System.Drawing.Size(277, 251);
            this.listPrivateConversation.TabIndex = 1;
            // 
            // listOfConversations
            // 
            this.listOfConversations.FormattingEnabled = true;
            this.listOfConversations.Location = new System.Drawing.Point(6, 5);
            this.listOfConversations.Name = "listOfConversations";
            this.listOfConversations.Size = new System.Drawing.Size(277, 303);
            this.listOfConversations.TabIndex = 0;
            this.listOfConversations.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listOfConversations_MouseDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(108, 144);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Username:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(105, 186);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "Login";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(105, 160);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 11;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 355);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "Chat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabChatroom.ResumeLayout(false);
            this.tabChatroom.PerformLayout();
            this.tabPrivate.ResumeLayout(false);
            this.tabPrivate.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabChatroom;
        private System.Windows.Forms.TabPage tabPrivate;
        private System.Windows.Forms.ListBox listOfConversations;
        private System.Windows.Forms.Button sendButtonChat;
        private System.Windows.Forms.Button backButtonChat;
        private System.Windows.Forms.TextBox textTypingChat;
        private System.Windows.Forms.ListBox listChatroom;
        private System.Windows.Forms.Button sendButtonPrivate;
        private System.Windows.Forms.Button backButtonPrivate;
        private System.Windows.Forms.TextBox textTypingPrivate;
        private System.Windows.Forms.ListBox listPrivateConversation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
    }
}

