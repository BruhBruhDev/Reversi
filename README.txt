
This is a school project which I made in a timeframe of a week. If you find any bugs let me know!

The main code is located in Form1.cs
You can find the compiled executable in:
bin/Debug/Graphics Program Reversi.exe


About the game - Reversi :
(it is similar to Thrillo)
There are 2 players and the goal is to have the most tiles of your own
colour as soon as the opponent is unable to place any more of his colour.

At the start there are 2 tiles of both players lined up in the center with this pattern:
A B
B A

You can place a tile on a certain empty field if you have, looking from that position,
at least one diagonal, horizontal or vertical line full of your opponents
colour up to one of your own.
Then you make your move, all in between oponnents colour tiles will switch their colour up to the
first encounter of your tile colour.

To better get your head around that, an example:
Here you are A and shown below is an extract of a possible alignment in a running game. To keep it simple only a horizontal line is shown here.
    1 2 3 4 5 6 7 8 9
    _ _ B B B A _ _ A
You can place your tile at the second position,
because on the horizontal direction a tile of your colour (A) is located after the 3 Bs at position 6
Following this move, every B in between will change colour
    1 2 3 4 5 6 7 8 9
    _ A A A A A _ _ A

This concept applies for horizontal, vertical and diagonal directions.

If you didn't quite understand that yet, you will probably figure it out once you have played it.
There is also a highlight move feature you can toggle.
As mentioned above, you'll find the EXE here:
bin/Debug/Graphics Program Reversi.exe
