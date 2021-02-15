using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Graphics_Program_Reversi
{
	/* Changes after reaching in project:
	 * 
	 * Noticed bugs:
	 *  - if a team wins by being the only team remaining:
	 *               the winner team is mixed up with the other one in the prompt
	 *               (fixed)
	 *  - @FlipRay: if the opponents tiles are lined up to the end of the board; they will all flip
	 *               variable badcase's usage had an unconsidered case
	 *               (fixed: the usage of badcase was inverted --> now it is always true and is only changed to false if the only needed case is met)
	 * Code changes:
	 *  - @CheckIfGameEnded: made more use of the DATA.TeamValue enum
	 * 
	 * Naming changes:
	 *  - HighlightAllPossibleMoves --> HighlightAllPossibleOctaMoves
	 *  - HighlightPossibleMoves --> HighlightPossibleOctaMoves
	 *  - HighlightPossibleMovesSUB1 --> HighlightPossibleMoves
	 *  - FlipRays --> FlipOctaRay
	 *  - FlipRaysSUB1 --> FlipRay
	 *  
	 *  - int end @CheckIfGameEnded --> winnerteam
	*/

	public partial class Form1 : Form
	{
		public Form1()
		{
			Methods.RESET();
			InitializeComponent();
		}
		
		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			Graphics G = e.Graphics;
			
			int wind_x = this.ClientSize.Width;
				DATA.wind_x = wind_x;
			int wind_y = this.ClientSize.Height;
				DATA.wind_y = wind_y;

			int gridsize = DATA.gridSize;
			int edge = DATA.edge;
			int tileSize = ((wind_x < wind_y ? wind_x : wind_y) - 2 * edge) / gridsize;
				DATA.tileSize = tileSize;
			if (tileSize < 0) return; // either you set your window to a decent size or you'll get nothing! (in order to prevent glitchy canvas)


			bool colorSwitch = false, colorSwitch2 = gridsize % 2 == 0;
			int x , y;
			int sizeIn = -4, sizeOut = -3;
			for (int iX = 0; iX < gridsize; iX++)
			{
				if (colorSwitch2) colorSwitch = !colorSwitch;
				for (int iY = 0; iY < gridsize; iY++)
				{
					x = iX * tileSize + edge; y = iY * tileSize + edge;
					G.FillRectangle(colorSwitch ? Brushes.Green : Brushes.ForestGreen, x, y, tileSize, tileSize);
					colorSwitch = !colorSwitch;
						DATA.tileCoords[iX, iY].X = x;
						DATA.tileCoords[iX, iY].Y = y;

					x -= 1; y -= 1; // adjusts the 1-pxl error of the Ellipses; together with the +1s spread in the switch below
					switch (DATA.tileTeamValues[iX, iY])
					{
						case (int)DATA.TeamValue.Empty:
							break;
						case (int)DATA.TeamValue.Black:
							G.FillEllipse(Brushes.DimGray, x - sizeOut, y - sizeOut, tileSize + 1 + sizeOut * 2, tileSize + 1 + sizeOut * 2);
							G.FillEllipse(Brushes.Black, x - sizeIn, y - sizeIn, tileSize + 1 + sizeIn * 2, tileSize + 1 + sizeIn * 2);
							break;
						case (int)DATA.TeamValue.White:
							G.FillEllipse(Brushes.DimGray, x - sizeOut, y - sizeOut, tileSize + 1 + sizeOut * 2, tileSize + 1 + sizeOut * 2);
							G.FillEllipse(Brushes.LightGray, x - sizeIn, y - sizeIn, tileSize + 1 + sizeIn * 2, tileSize + 1 + sizeIn * 2);
							break;
					}
				}
			}

			// text display
			string displayedText = "Team " + ( DATA.currentTeam == 1 ? "Black" : "White") + "'s turn";
			SizeF stringWidth = G.MeasureString(displayedText,Font);
			G.DrawString(displayedText, Font, Brushes.White, (wind_x - stringWidth.Width) / 2, 2);

			// borders aka highlight
			if (DATA.doHighlight)
            {
				int borderWidth = 1 + tileSize / 30; // actual width is borderwidth * 2
				for (int iA = 0; iA < gridsize; iA++)
				{
					for (int iB = 0; iB < 1 + gridsize; iB++)
					{
						//vertical borders
						if ((iB - 1 >= 0 && DATA.tileMarked[iB - 1, iA]) || (iB < gridsize && DATA.tileMarked[iB, iA]))
						{
							x = tileSize * iB + edge; y = tileSize * iA + edge;
							G.FillRectangle(Brushes.White, x - borderWidth, y, borderWidth * 2 - 1, tileSize - 1);
						}
						//horizontal borders
						if ((iB - 1 >= 0 && DATA.tileMarked[iA, iB - 1]) || (iB < gridsize && DATA.tileMarked[iA, iB]))
						{
							x = tileSize * iA + edge; y = tileSize * iB + edge;
							G.FillRectangle(Brushes.White, x, y - borderWidth, tileSize - 1, borderWidth * 2 - 1);
						}
					}
				}
			}
			
		}
		private void Form1_MouseClick(object sender, MouseEventArgs e)
		{
			if (!Methods.IsClickInside(e.Location)) return;
			Point pos = Methods.GetTileAtPos(e.Location);
			if (DATA.tileMarked[pos.X, pos.Y])
			{
				DATA.tileTeamValues[pos.X, pos.Y] = DATA.currentTeam;
				Methods.FlipOctaRay(pos);
				// team switch
				if (DATA.currentTeam == (int)DATA.TeamValue.White) DATA.currentTeam = (int)DATA.TeamValue.Black;
				else if (DATA.currentTeam == (int)DATA.TeamValue.Black) DATA.currentTeam = (int)DATA.TeamValue.White;

				Methods.GridClearHighlight();
				Methods.HighlightAllPossibleOctaMoves();
				
				Refresh();
				
				Methods.CheckIfGameEnded();
				Refresh();
			}
						
		}
		private void Form1_ClientSizeChanged(object sender, EventArgs e)
		{
			Refresh();
		}
		private void button_highlight_Click(object sender, EventArgs e)
		{
			DATA.doHighlight = !DATA.doHighlight;
			Refresh();
		}
		private void button_restart_Click(object sender, EventArgs e)
		{
			Methods.RESET();
			Refresh();
		}

		// ----- -----  ----- -----  ----- -----  ----- -----  ----- -----  ----- -----  ----- -----  ----- -----  

		static class DATA
		{
			public enum TeamValue
			{
				Empty,
				Black,
				White,
				Both
			}

			public static int gridSize = 8; // amount of tiles (squared)
			public static int edge = 40; // width in pixels for the edge
			
			public static int wind_x = 0, wind_y = 0, tileSize = 0; // are updated during runtime (value in pixels)
			public static int amountOfMovesForCurrentTeam = 0; // updated during runtime
			/// <summary>
			/// Stores values found in the DATA.TeamValue enumerate
			/// </summary>
			public static int[,] tileTeamValues = new int[gridSize, gridSize];
			public static Point[,] tileCoords = new Point[gridSize, gridSize];
			public static bool[,] tileMarked = new bool[gridSize, gridSize];
			public static int currentTeam = (int)TeamValue.Black;
			public static bool doHighlight = false;
		}

		// ----- -----  ----- -----  ----- -----  ----- -----  ----- -----  ----- -----  ----- -----  ----- -----  

		static class Methods
		{
			public static bool IsClickInside(Point pos)
			{
				return pos.X >= DATA.tileCoords[0, 0].X
					&& pos.Y >= DATA.tileCoords[0, 0].Y
					&& pos.X < DATA.tileCoords[DATA.gridSize - 1, DATA.gridSize - 1].X + DATA.tileSize
					&& pos.Y < DATA.tileCoords[DATA.gridSize - 1, DATA.gridSize - 1].Y + DATA.tileSize;
			}
			public static Point GetTileAtPos(Point pos)
			{
				int tileSize = DATA.tileSize, tCx = DATA.tileCoords[0, 0].X, tCy = DATA.tileCoords[0, 0].Y;
				Point pt = new Point(
					// Formula: moves grid to zero and divides the new upper-grid-end by the tilesize.
					// Additionally decides if it is negative and adjusts the value by -1.
					pos.X - tCx >= 0 ? (pos.X - tCx) / tileSize : (pos.X - tCx) / tileSize - 1,
					pos.Y - tCy >= 0 ? (pos.Y - tCy) / tileSize : (pos.Y - tCy) / tileSize - 1);
				return pt;
			}
			public static void HighlightAllPossibleOctaMoves()
			{
				DATA.amountOfMovesForCurrentTeam = 0;
				for (int x = 0; x < DATA.gridSize; x++)
					for (int y = 0; y < DATA.gridSize; y++)
						if (DATA.tileTeamValues[x, y] == DATA.currentTeam)
							HighlightPossibleOctaMoves(new Point(x, y));
			}
			private static void HighlightPossibleOctaMoves(Point pos)
			{
				List<Point> path;
                for (int i = 0; i < 16; i += 2) // goes through every path
                {
					path = HighlightPossibleMoves(pos, VAR_directions[i], VAR_directions[i+1]);
					if (path.Count != 0)
                        for (int i2 = path.Count-1; i2 >= 0; i2--) // goes through the currently indexed path
                        {
							DATA.amountOfMovesForCurrentTeam++;
							DATA.tileMarked[path[i2].X, path[i2].Y] = true;
							break;
                        }
				}
			}
			private static readonly int[] VAR_directions = new int[16] {
				1, 0, 0, 1,
				-1, 0, 0, -1,
				1, 1, 1, -1,
				-1, 1, -1, -1};
			private static List<Point> HighlightPossibleMoves(Point pos, int stepX, int stepY)
            {
				int gridsize = DATA.gridSize; bool badcase = false, finish = false;
				List<Point> path = new List<Point>();
				for (int i = 1; i < gridsize - 1; i++)
				{
					if (pos.X + i * stepX >= 0 && pos.X + i * stepX < gridsize
						&& pos.Y + i * stepY >= 0 && pos.Y + i * stepY < gridsize)
						if (DATA.tileTeamValues[pos.X + i * stepX, pos.Y + i * stepY] == (int)DATA.TeamValue.Empty)
						{
							path.Add(new Point(pos.X + i * stepX, pos.Y + i * stepY));
							finish = true; break;
						}
						else if (DATA.tileTeamValues[pos.X + i * stepX, pos.Y + i * stepY] != DATA.currentTeam)
						{
							path.Add(new Point(pos.X + i * stepX, pos.Y + i * stepY));
						}
						else { badcase = true; break; }
                }
				if (badcase || !finish || path.Count == 1) path.Clear();
				return path;
            }
			public static void FlipOctaRay(Point pos)
			{
				List<Point> path;
				int selectedTeam = DATA.tileTeamValues[pos.X, pos.Y];
				for (int i = 0; i < 16; i += 2) // goes through every path
				{
					path = FlipRay(pos, VAR_directions[i], VAR_directions[i + 1]);
					for (int i2 = 0; i2 < path.Count; i2++) // goes through the currently indexed path	
						DATA.tileTeamValues[path[i2].X, path[i2].Y] = selectedTeam;
					
				}
			}
			private static List<Point> FlipRay(Point pos, int stepX, int stepY)
            {
				int gridsize = DATA.gridSize, selectedTeam = DATA.tileTeamValues[pos.X, pos.Y]; bool badcase = true;
				List<Point> path = new List<Point>();
				for (int i = 1; i < gridsize - 1; i++)
				{
					if (pos.X + i * stepX >= 0 && pos.X + i * stepX < gridsize
						&& pos.Y + i * stepY >= 0 && pos.Y + i * stepY < gridsize)
						if (DATA.tileTeamValues[pos.X + i * stepX, pos.Y + i * stepY] == (int)DATA.TeamValue.Empty)
							break;
						else if (DATA.tileTeamValues[pos.X + i * stepX, pos.Y + i * stepY] == selectedTeam)
							{ badcase = false; break; }
						else if (DATA.tileTeamValues[pos.X + i * stepX, pos.Y + i * stepY] != selectedTeam)
							path.Add(new Point(pos.X + i * stepX, pos.Y + i * stepY));
				}
				if (badcase) path.Clear();
				return path;
			}
			public static void GridClearHighlight()
			{
				for (int x = 0; x < DATA.gridSize; x++)
					for (int y = 0; y < DATA.gridSize; y++)
						DATA.tileMarked[x, y] = false;
			}
			public static void CheckIfGameEnded()
            {
				int black = 0, white = 0;
                for (int x = 0; x < DATA.gridSize; x++)
                    for (int y = 0; y < DATA.gridSize; y++)
						if (DATA.tileTeamValues[x, y] == (int)DATA.TeamValue.Black) black++;
						else if (DATA.tileTeamValues[x, y] == (int)DATA.TeamValue.White) white++;
				int winnerTeam = (int)DATA.TeamValue.Empty;
				if (black == 0) winnerTeam = (int)DATA.TeamValue.White;
				else if (white == 0) winnerTeam = (int)DATA.TeamValue.Black;
				else if (black + white == DATA.gridSize * DATA.gridSize)
                {
					if (black == white) winnerTeam = (int)DATA.TeamValue.Both;
					else if (black > white) winnerTeam = (int)DATA.TeamValue.Black;
					else if (black < white) winnerTeam = (int)DATA.TeamValue.White;
				}
				else if (DATA.amountOfMovesForCurrentTeam == 0)
						if (DATA.currentTeam == (int)DATA.TeamValue.Black) winnerTeam = (int)DATA.TeamValue.White;
						else if (DATA.currentTeam == (int)DATA.TeamValue.White) winnerTeam = (int)DATA.TeamValue.Black;
				if (winnerTeam != (int)DATA.TeamValue.Empty)
					switch (MessageBox.Show("Game ended: "+
						(winnerTeam == (int)DATA.TeamValue.Black ? "Black won!" : winnerTeam == (int)DATA.TeamValue.White ? "White won!" : "Draw!"),
						"Select something. Play again?", MessageBoxButtons.YesNo))
					{
						case DialogResult.Yes:
							RESET();
							break;
						case DialogResult.No:
							break;
					}

			}
			public static void RESET()
			{
				if (DATA.gridSize >= 5)
				{
					for (int x = 0; x < DATA.gridSize; x++)
						for (int y = 0; y < DATA.gridSize; y++)
							DATA.tileTeamValues[x, y] = (int)DATA.TeamValue.Empty;
					DATA.tileTeamValues[3, 3] = 1;
					DATA.tileTeamValues[3, 4] = 2;
					DATA.tileTeamValues[4, 3] = 2;
					DATA.tileTeamValues[4, 4] = 1;
					DATA.currentTeam = (int)DATA.TeamValue.Black;
					GridClearHighlight();
					HighlightAllPossibleOctaMoves();
				}
			}
		}
    }
}
