using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleShip
{

    // types of cells (clear, ship, etc.).
    enum CellType
    {
        empty,
        ship,
        test_ship,
        miss,
        destroyed
    }
    public partial class Form1 : Form
    {
        Player current = null;
        Player enemy = null;
        Player p1 = null;
        Player p2 = null;

        // field size.
        const int size = 10;

        // mode:
        // 0 - start,
        // 1 - first player places ships,
        // 2 - second player places ships,
        // 3 - player moves,
        // 4 - move changes.
        int mode = 0;

        // empty field.
        CellType[,] empty_field = new CellType[size, size];


        // clears field and ships.
        void ClearField(CellType[,] field, int field_size)
        {
            for (int i = 0; i < field_size; i++)
            {
                for (int j = 0; j < field_size; j++)
                {
                    field[i, j] = CellType.empty;
                }
            }
        }


        // remake field into game.
        void RestoreField(CellType[,] field, int field_size)
        {
            for (int i = 0; i < field_size; i++)
            {
                for (int j = 0; j < field_size; j++)
                {
                    if (field[i, j] == CellType.test_ship)
                        field[i, j] = CellType.ship;
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        // gets cell's type.
        CellType GetCell(CellType[,] field, int field_size, int row, int col)
        {
            if (row < 0 || col < 0 || row >= field_size || col >= field_size)
            {
                // if cell is out of range.

                return CellType.empty;
            }
            return field[row, col];
        }

        // checks field and returns:
        // -1 if the alignment is incorrect;
        //  0 if there is not any ships on field;
        //  1 if everything is ok;
        //  2 if there is not any destroyed ships.
        int CheckField(CellType[,] field, int field_size)
        {
            // types of ships.
            int n1 = 0;
            int n2 = 0;
            int n3 = 0;
            int n4 = 0;
            for (int i = 0; i < field_size; i++)
            {
                for (int j = 0; j < field_size; j++)
                {
                    // if the ship was found.
                    if (field[i, j] == CellType.ship)
                    {
                        if (j < field_size - 1 && field[i, j + 1] == CellType.ship)
                        {
                            // horiz. orient.
                            int k = j;
                            // end of ship serching.
                            while (GetCell(field, field_size, i, k) == CellType.ship)
                            {
                                // replace for test.
                                field[i, k] = CellType.test_ship;
                                if (GetCell(field, field_size, i - 1, k) != CellType.empty || GetCell(field, field_size, i + 1, k) != CellType.empty)
                                {
                                    // if there is any "not clear" cells near the ship.
                                    RestoreField(field, field_size);
                                    return -1;
                                }
                                k++;
                            }
                            // if all cells near the ship are clear.
                            if (GetCell(field, field_size, i - 1, j - 1) == CellType.empty &&
                                GetCell(field, field_size, i, j - 1) == CellType.empty &&
                                GetCell(field, field_size, i + 1, j - 1) == CellType.empty &&
                                GetCell(field, field_size, i - 1, k) == CellType.empty &&
                                GetCell(field, field_size, i, k) == CellType.empty &&
                                GetCell(field, field_size, i + 1, k) == CellType.empty)
                            {
                                // types of ships counting.
                                switch (k - j)
                                {
                                    case 2:
                                        n2++;
                                        break;
                                    case 3:
                                        n3++;
                                        break;
                                    case 4:
                                        n4++;
                                        break;
                                    default:
                                        {
                                            // if the length is more then 4.
                                            RestoreField(field, field_size);
                                            return -1;
                                        }
                                }
                            }
                            else
                            {
                                // if any cells near the ship are not empty.
                                RestoreField(field, field_size);
                                return -1;
                            }
                        }
                        else if (i < field_size - 1 && field[i + 1, j] == CellType.ship)
                        {
                            // vertic. orient.
                            int k = i;
                            //searching for the end of ship.
                            while (GetCell(field, field_size, k, j) == CellType.ship)
                            {
                                // replace for test.
                                field[k, j] = CellType.test_ship;

                                // if there is any "not clear" cells near the ship.
                                if (GetCell(field, field_size, k, j - 1) != CellType.empty || GetCell(field, field_size, k, j + 1) != CellType.empty)
                                {
                                    RestoreField(field, field_size);
                                    return -1;
                                }
                                k++;
                            }

                            // if everything is ok.
                            if (GetCell(field, field_size, i - 1, j - 1) == CellType.empty &&
                                GetCell(field, field_size, i - 1, j) == CellType.empty &&
                                GetCell(field, field_size, i - 1, j + 1) == CellType.empty &&
                                GetCell(field, field_size, k, j - 1) == CellType.empty &&
                                GetCell(field, field_size, k, j) == CellType.empty &&
                                GetCell(field, field_size, k, j + 1) == CellType.empty)
                            {
                                // types of ships counting.
                                switch (k - i)
                                {
                                    case 2:
                                        n2++;
                                        break;
                                    case 3:
                                        n3++;
                                        break;
                                    case 4:
                                        n4++;
                                        break;
                                    default:
                                        {
                                            // if kength is more then 4.
                                            RestoreField(field, field_size);
                                            return -1;
                                        }
                                }
                            }
                            else
                            {
                                // if any cells near thi ship are not empty.
                                RestoreField(field, field_size);
                                return -1;
                            }
                        }
                        else
                        {
                            // one-cell ship.
                            if (GetCell(field, field_size, i - 1, j) == CellType.empty &&
                                GetCell(field, field_size, i - 1, j - 1) == CellType.empty &&
                                GetCell(field, field_size, i - 1, j + 1) == CellType.empty &&
                                GetCell(field, field_size, i, j - 1) == CellType.empty &&
                                GetCell(field, field_size, i, j + 1) == CellType.empty &&
                                GetCell(field, field_size, i + 1, j) == CellType.empty &&
                                GetCell(field, field_size, i + 1, j - 1) == CellType.empty &&
                                GetCell(field, field_size, i + 1, j + 1) == CellType.empty)
                            {
                                n1++;
                                field[i, j] = CellType.test_ship;
                            }
                            else
                            {
                                RestoreField(field, field_size);
                                return -1;
                            }
                        }
                    }
                }
            }
            RestoreField(field, field_size);

            // if there are all ships on field
            if (n1 == 4 && n2 == 3 && n3 == 2 && n4 == 1)
            {
                return 1;
            }
            else if (n1 == 0 && n2 == 0 && n3 == 0 && n4 == 0)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }

        // ships' placement on the field.
        CellType[,] ship_generate(int field_size)
        {
            Random rnd = new Random();

            // creating new field.
            CellType[,] ap_data = new CellType[field_size, field_size];

            // clearing a field.
            ClearField(ap_data, field_size);
            int count_ship = 1;

            // the process of ships' placement.
            while (count_ship <= 4)
            {
                for (int current_ship = 0; current_ship < count_ship;)
                {
                    // searchig for first cell of placement.
                    int x = rnd.Next(field_size);
                    int y = rnd.Next(field_size);

                    // searching for direction.
                    int dir = rnd.Next(4);
                    bool setting_is_possible = true;

                    for (int i = 0; i < 5 - count_ship; i++)
                    {
                        // if out of range.
                        if (x < 0 || y < 0 || x >= field_size || y >= field_size)
                        {
                            setting_is_possible = false;
                            break;
                        }
                        // if another ship disturbs.
                        if (ap_data[y, x] == CellType.ship)
                        {
                            setting_is_possible = false;
                            break;
                        }

                        // if another ship is near.
                        if (y < field_size - 1 && ap_data[(y + 1), x] == CellType.ship)
                        {
                            setting_is_possible = false;
                            break;
                        }
                        // searching for any ships near the chosen ship.
                        if (y > 0 && ap_data[(y - 1), x] == CellType.ship)
                        {
                            setting_is_possible = false;
                            break;
                        }
                        if (x < field_size - 1 && ap_data[y, x + 1] == CellType.ship)
                        {
                            setting_is_possible = false;
                            break;
                        }
                        if (x < field_size - 1 && y < field_size - 1 && ap_data[y + 1, x + 1] == CellType.ship)
                        {
                            setting_is_possible = false;
                            break;
                        }

                        if (y > 0 && x < field_size - 1 && ap_data[y - 1, x + 1] == CellType.ship)
                        {
                            setting_is_possible = false;
                            break;
                        }

                        if (x > 0 && ap_data[y, x - 1] == CellType.ship)
                        {
                            setting_is_possible = false;
                            break;
                        }
                        if (y < field_size - 1 && x > 0 && ap_data[y + 1, x - 1] == CellType.ship)
                        {
                            setting_is_possible = false;
                            break;
                        }
                        if (x > 0 && y > 0 && ap_data[y - 1, x - 1] == CellType.ship)
                        {
                            setting_is_possible = false;
                            break;
                        }

                        // if we can place the ship, we place test.
                        if (setting_is_possible)
                        {
                            ap_data[y, x] = CellType.test_ship;
                        }

                        // direction.
                        switch (dir)
                        {
                            case 0:
                                x++;
                                break;

                            case 1:
                                y++;
                                break;

                            case 2:
                                x--;
                                break;

                            case 3:
                                y--;
                                break;
                        }
                    }
                    // if the whole ship was placed.
                    if (setting_is_possible)
                    {
                        current_ship++;

                        //replace from test to real.
                        RestoreField(ap_data, field_size);
                    }
                    else
                    {
                        for (int i = 0; i < field_size; i++)
                        {
                            for (int j = 0; j < field_size; j++)
                            {
                                if (ap_data[i, j] == CellType.test_ship)
                                    ap_data[i, j] = CellType.empty;
                            }
                        }
                    }
                }
                count_ship++;
            }
            return ap_data;
        }

        // the main button.
        private void button1_Click(object sender, EventArgs e)
        {
            switch (mode)
            {
                // when the game starts.
                case 0:
                    // creating the first player.
                    p1 = new Player(size, 1);
                    current = p1;
                    label1.Text = "The first player places the ships";
                    Graphics g = pictureBox2.CreateGraphics();
                    // clearing second player's field.
                    g.Clear(SystemColors.Control);
                    button1.Text = "Ready";
                    button2.Visible = true;
                    mode++;


                    // draw first player's field.
                    DrawField(pictureBox1, current.field, size);
                    break;

                // first player places ships.
                case 1:
                    if (CheckField(current.field, size) == 1)
                    {
                        // create second player.
                        p2 = new Player(size, 2);
                        current = p2;
                        label1.Text = "The second player places the ships";
                        mode++;
                        DrawField(pictureBox1, current.field, size);
                    }
                    else
                    {
                        MessageBox.Show("Àrrange the ships correctly");
                    }
                    break;

                // second player places ships.
                case 2:
                    if (CheckField(current.field, size) == 1)
                    {
                        button2.Visible = false;
                        current = p1;
                        enemy = p2;
                        label1.Text = String.Format("Player number {0} moves", current.num);
                        DrawField(pictureBox1, current.field, size);
                        DrawField(pictureBox2, current.enemy_field, size);
                        mode++;
                    }
                    else
                    {
                        MessageBox.Show("Àrrange the ships correctly");
                    }
                    break;

                // the move changes.
                case 4:
                    label1.Text = String.Format("Player number {0} moves", current.num);
                    DrawField(pictureBox1, current.field, size);
                    DrawField(pictureBox2, current.enemy_field, size);
                    mode--;
                    break;
            }
        }

        // second button (random generation)
        private void button2_Click(object sender, EventArgs e)
        {
            if (mode == 1 || mode == 2)
            {
                current.field = ship_generate(size);
                DrawField(pictureBox1, current.field, size);
            }
        }

        // makes field.
        void DrawField(PictureBox pb, CellType[,] field, int field_size)
        {
            Pen pen = new Pen(Color.Black, 1);
            Pen redpen = new Pen(Color.Red, 2);
            Brush brush = new SolidBrush(Color.Black);
            Brush bluebrush = new SolidBrush(Color.Blue);
            Brush whitebrush = new SolidBrush(Color.White);
            Graphics g = pb.CreateGraphics();

            // calculating sizes of cell.
            int cell_width = pb.Width / field_size;
            int cell_height = pb.Height / field_size;

            // field cells drawing.
            for (int i = 0; i <= field_size; i++)
            {
                g.DrawLine(pen, i * cell_width, 0, i * cell_width, pb.Height);
                g.DrawLine(pen, 0, i * cell_height, pb.Width, i * cell_height);
            }

            // first and second players' fields drawing.
            for (int i = 0; i < field_size; i++)
            {
                for (int j = 0; j < field_size; j++)
                {
                    switch (field[i, j])
                    {
                        case CellType.empty:
                            g.FillRectangle(whitebrush, j * cell_width + 1, i * cell_height + 1, cell_width - 1, cell_height - 1);
                            break;
                        case CellType.ship:
                            g.FillRectangle(brush, j * cell_width, i * cell_height, cell_width, cell_height);
                            break;
                        case CellType.destroyed:
                            g.DrawLine(redpen, j * cell_width, i * cell_height, (j + 1) * cell_width, (i + 1) * cell_height);
                            g.DrawLine(redpen, (j + 1) * cell_width, i * cell_height, (j) * cell_width, (i + 1) * cell_height);
                            break;
                        case CellType.miss:
                            g.FillEllipse(bluebrush, j * cell_width + cell_width / 4, i * cell_height + cell_height / 4, cell_width / 2, cell_height / 2);
                            break;
                    }
                }
            }
        }

        // clicks on first player's field.
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (mode == 1 || mode == 2)
            {
                // calculating chosen cell.
                int cell_width = pictureBox1.Width / size + 1;
                int cell_height = pictureBox1.Height / size + 1;
                int row = e.Y / cell_height;
                int col = e.X / cell_width;

                if (current.field[row, col] == CellType.ship)
                {
                    current.field[row, col] = CellType.empty;
                }
                else
                {
                    current.field[row, col] = CellType.ship;
                    if (CheckField(current.field, size) == -1)
                    {
                        current.field[row, col] = CellType.empty;
                    }
                }
                DrawField(pictureBox1, current.field, size);

            }
        }

        // clicks on second player's field.
        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            if (mode == 3)
            {
                int cell_width = pictureBox2.Width / size + 1;
                int cell_height = pictureBox2.Height / size + 1;
                int row = e.Y / cell_height;
                int col = e.X / cell_width;

                if (current.enemy_field[row, col] == CellType.empty)
                {
                    if (enemy.field[row, col] == CellType.ship)
                    {
                        // mark the ships we hit.
                        current.enemy_field[row, col] = CellType.destroyed;
                        enemy.field[row, col] = CellType.destroyed;
                        int n = 0;

                        // calculating for ships' length
                        for (int k = row; GetCell(enemy.field, size, k, col) != CellType.empty; k++)
                            if (GetCell(enemy.field, size, k, col) == CellType.ship)
                                n++;
                        for (int k = row; GetCell(enemy.field, size, k, col) != CellType.empty; k--)
                            if (GetCell(enemy.field, size, k, col) == CellType.ship)
                                n++;
                        for (int k = col; GetCell(enemy.field, size, row, k) != CellType.empty; k++)
                            if (GetCell(enemy.field, size, row, k) == CellType.ship)
                                n++;
                        for (int k = col; GetCell(enemy.field, size, row, k) != CellType.empty; k--)
                            if (GetCell(enemy.field, size, row, k) == CellType.ship)
                                n++;

                        // if not destroyed
                        if (n > 0)
                        {
                            label1.Text = "Hit!";
                        }
                        else
                        {
                            // if there are no full ships.
                            label1.Text = "Destroyed!";
                            if (CheckField(enemy.field, size) == 2)
                            {
                                // victory
                                MessageBox.Show(String.Format("Player number {0} wins", current.num));
                                mode = 0;
                                button1.Text = "New Game";
                                label1.Text = "";
                            }
                        }

                        DrawField(pictureBox2, current.enemy_field, size);

                    }
                    else
                    {
                        // miss.
                        current.enemy_field[row, col] = CellType.miss;
                        label1.Text = String.Format("Miss! Player number {0} is getting ready", enemy.num);
                        mode++;
                        DrawField(pictureBox1, empty_field, size);
                        DrawField(pictureBox2, empty_field, size);
                        Player tmp = current;
                        current = enemy;
                        enemy = tmp;
                    }
                }
            }
        }

        // reload field while new form opens.
        private void Form1_Load(object sender, EventArgs e)
        {
            ClearField(empty_field, size);
        }

        // image update.
        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (mode)
            {
                case 1:
                case 2:
                    {
                        DrawField(pictureBox1, current.field, size);
                        break;
                    }
                case 3:
                    {
                        DrawField(pictureBox1, current.field, size);
                        DrawField(pictureBox2, current.enemy_field, size);
                        break;
                    }
                case 4:
                    {
                        DrawField(pictureBox1, empty_field, size);
                        DrawField(pictureBox2, empty_field, size);
                        break;
                    }

            }
        }
    }


    // player's class settings.
    class Player
    {
        public CellType[,] field, enemy_field;
        public int num; // player's num
        int field_size;
        public Player(int fs, int n)
        {
            field_size = fs;
            num = n;
            field = new CellType[field_size, field_size];
            enemy_field = new CellType[field_size, field_size];
            for (int i = 0; i < field_size; i++)
            {
                for (int j = 0; j < field_size; j++)
                {
                    field[i, j] = CellType.empty;
                    enemy_field[i, j] = CellType.empty;
                }
            }
        }
    }

}