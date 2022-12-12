using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Board : MonoBehaviour
{
    [SerializeField]
    GameObject piece;
    [SerializeField]
    GameObject crown;
    [SerializeField]
    GameObject selection;
    [SerializeField]
    GameObject dot;
    [SerializeField]
    GameObject cross;

    bool gameOver = false;
    [SerializeField]
    GameObject gameOverDisplay1;
    [SerializeField]
    GameObject gameOverDisplay2;

    Dictionary<Vector2Int, GameObject> pieces = new Dictionary<Vector2Int, GameObject>();

    GameState gameState;
    Forecast forecast;
    side playerSide = side.white;

    static public Vector2Int[] kingDirections = new Vector2Int[]{new Vector2Int(-1, -1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(1, 1)};
    static public Vector2Int[] playerDirections = new Vector2Int[]{new Vector2Int(-1, 1), new Vector2Int(1, 1)};
    static public Vector2Int[] opponentDirections = new Vector2Int[]{new Vector2Int(-1, -1), new Vector2Int(1, -1)};

    static Vector2Int INVALIDSQUARE = new Vector2Int(-1,-1);

    [SerializeField]
    Vector2Int selectedFirst = INVALIDSQUARE;
    [SerializeField]
    Vector2Int selectedLast = INVALIDSQUARE;
    [SerializeField]
    List<Vector2Int> selectedJumps = new List<Vector2Int>();
    [SerializeField]
    List<Vector2Int> validMoves = new List<Vector2Int>();
    [SerializeField]
    Vector2Int[] validDirections;

    List<GameObject> validMovesDisplay = new List<GameObject>();
    List<GameObject> selectedDisplay = new List<GameObject>();
    List<GameObject> moveDisplay = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        //initialize gamestate and generate move forecast
        gameState = new GameState(playerSide == side.white);
        forecast = new Forecast(gameState);
        forecast.generate(5);

        //instantiate pieces
        for(int y=0; y < 3; y++)
        {
            for(int x=y%2; x < 8; x+=2)
            {
                GameObject newPiece = Instantiate(piece, getRealSpace(new Vector2Int(x, y)), Quaternion.identity);
                if (playerSide == side.white)
                {
                    newPiece.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                }
                else
                {
                    newPiece.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                }
                pieces.Add(new Vector2Int(x, y), newPiece);
            }
        }

        for(int y=5; y < 8; y++)
        {
            for(int x=y%2; x < 8; x+=2)
            {
                GameObject newPiece = Instantiate(piece, getRealSpace(new Vector2Int(x, y)), Quaternion.identity);
                if (playerSide == side.black)
                {
                    newPiece.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                }
                else
                {
                    newPiece.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                }
                    pieces.Add(new Vector2Int(x, y), newPiece);
            }
        }
    }

    //get a world space position given a square on the board
    public static Vector3 getRealSpace(Vector2Int square)
    {
        return new Vector3(-3.5f+square.x, -3.5f+square.y, 0);
    }

    //get a square on the board given a real space position
    public static Vector2Int getSquare(Vector3 realSpace)
    {
        if (realSpace.x >= -4 && realSpace.y >= -4 && realSpace.x <= 4 && realSpace.y <= 4)
        {
            Vector2Int square = new Vector2Int((int)((realSpace.x+4)), (int)((realSpace.y+4)));
            if (square.x%2 == square.y%2)
            {
                return square;
            }
        }

        return INVALIDSQUARE;
    }

    //check that a square exists on the board ie neither coordinate is out of range
    public static bool isOnBoard(Vector2Int square)
    {
        return square.x >=0 && square.y >=0 && square.x < 8 && square.y < 8;
    }

    //get and display the valid moves a player can take with the selected piece
    void getValidMoves()
    {
        validMoves.Clear();
        //display jumps if possible
        if (!forecast.jumpPossible)
        {
            foreach (Vector2Int direction in validDirections)
            {
                Vector2Int square = selectedFirst + direction;
                if (isOnBoard(square))
                {
                    if (gameState.isEmpty(square))
                    {
                        validMoves.Add(square);
                    }
                }
            }
        }
        //display other moves if jump not possible
        else
        {
            foreach (Vector2Int direction in validDirections)
            {
                Vector2Int square = selectedLast + direction;
                if (isOnBoard(square))
                {
                    if (gameState.isOpponent(square) && !selectedJumps.Contains(square))//doesn't work because 
                    {
                        square += direction;
                        if (isOnBoard(square))
                        {
                            if (gameState.isEmpty(square) || square == selectedFirst)
                            {
                                validMoves.Add(square);
                            }
                        }
                    }
                }
            }
        }
    }

    //clear display of selected piece, previous move and valid moves
    void displayClear()
    {
        foreach(GameObject display in selectedDisplay)
        {
            Destroy(display);
        }
        selectedDisplay.Clear();
        foreach(GameObject display in validMovesDisplay)
        {
            Destroy(display);
        }
        validMovesDisplay.Clear();
        foreach(GameObject display in moveDisplay)
        {
            Destroy(display);
        }
        moveDisplay.Clear();
    }

    //display selected piece and selected jumps sofar
    void displaySelected(Vector2Int square)
    {
        if (selectedDisplay.Count > 0)
        {
            selectedDisplay.Add(Instantiate(dot, getRealSpace(square), Quaternion.identity));
        }
        else
        {
            selectedDisplay.Add(Instantiate(selection, getRealSpace(square), Quaternion.identity));
        }
    }

    //display valid moves for currently selected piece
    void displayValidMoves()
    {
        foreach(GameObject display in validMovesDisplay)
        {
            Destroy(display);
        }
        validMovesDisplay.Clear();
        foreach(Vector2Int move in validMoves)
        {
            GameObject newDot = Instantiate(dot, getRealSpace(move), Quaternion.identity);
            newDot.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1, 0.5f);
            validMovesDisplay.Add(newDot);
        }
    }

    //display previous move
    void displayMove(Move move)
    {
        GameObject newDot = Instantiate(dot, getRealSpace(move.from), Quaternion.identity);
        newDot.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0);;
        moveDisplay.Add(newDot);
        foreach (Vector2Int jump in move.jumps)
        {
            GameObject newCross = Instantiate(cross, getRealSpace(jump), Quaternion.identity);
            newCross.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0);;
            validMovesDisplay.Add(newCross);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if game is over display winner, and wait for player to click to return to menu
        if (forecast.gameOver){
            if (!gameOver)
            {
                gameOver = true;
                //if player has won add gold to inventory
                if (forecast.playerWins == true)
                {
                    Inventory.gold += 6;
                    gameOverDisplay1.GetComponent<TextMeshProUGUI>().enabled = true;
                    gameOverDisplay1.GetComponent<TextMeshProUGUI>().text = "You Won!";
                    gameOverDisplay2.GetComponent<TextMeshProUGUI>().enabled = true;
                }
                else
                {
                    gameOverDisplay1.GetComponent<TextMeshProUGUI>().enabled = true;
                    gameOverDisplay1.GetComponent<TextMeshProUGUI>().text = "You Lost!";
                    gameOverDisplay2.GetComponent<TextMeshProUGUI>().enabled = true;
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene("Menu");
            }
        }
        //if it is players turn process their inputs
        else if (gameState.isPlayersTurn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //if player has clicked square
                Vector2Int square = getSquare(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (square != INVALIDSQUARE)
                {
                    //if square contains players piece select piece and display valid moves
                    if (gameState.isPlayer(square))
                    {
                        selectedFirst = square;
                        selectedLast = selectedFirst;
                        if (gameState.isKing(square))
                        {
                            validDirections = kingDirections;
                        }
                        else
                        {
                            validDirections = playerDirections;
                        }

                        displayClear();

                        getValidMoves();

                        displayValidMoves();
                        displaySelected(square);
                    }
                    //if square is empty and player has piece selected check if piece can move into square
                    else if (gameState.isEmpty(square))
                    {
                        if (selectedFirst != INVALIDSQUARE)
                        {
                            //if move is valid, move piece
                            if (validMoves.Contains(square))
                            {
                                if (!forecast.jumpPossible)
                                {
                                    if (validMoves.Contains(square)){
                                        makeMove(new Move(selectedFirst, square));
                                        selectedFirst = INVALIDSQUARE;
                                        validMoves.Clear();
                                        //remove selected and validmove display
                                    }
                                }
                                else
                                {
                                    //if move is valid jump add to jumps
                                    if (validMoves.Contains(square)){
                                        selectedJumps.Add((selectedLast+square)/2);
                                        if (square.y == 7)
                                        {
                                            validDirections = kingDirections;
                                        }                                
                                        selectedLast = square;
                                        getValidMoves();
                                        displaySelected(square);
                                        displayValidMoves();
                                        //if cannot make any more jumps move piece
                                        if (validMoves.Count == 0)
                                        {
                                            makeMove(new Move(selectedFirst, selectedLast, selectedJumps));
                                            selectedFirst = INVALIDSQUARE;
                                            selectedLast = INVALIDSQUARE;
                                            selectedJumps.Clear();
                                            validMoves.Clear();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //if it is opponents turn generate move forecast and use minimax to choose move
        else
        {
            forecast.generate(5);

            List<Move> moves = forecast.evaluate();
            Move move = moves[Random.Range(0, moves.Count-1)];
            makeMove(move);
        }
    }

    //make move, updating gamestate and forecast
    void makeMove(Move move)
    {
        bool wasKing = gameState.isKing(move.from);

        displayClear();
        displayMove(move);

        //update forecast and gamestate
        forecast = forecast.getForecast(move);

        gameState = forecast.gameState;

        //move piece
        pieces[move.to] = pieces[move.from];

        pieces.Remove(move.from);

        pieces[move.to].transform.position = getRealSpace(move.to);

        //remove taken pieces
        foreach (Vector2Int jump in move.jumps)
        {
            Destroy(pieces[jump]);
            pieces.Remove(jump);
        }

        //if piece became king, add crown
        if (wasKing != gameState.isKing(move.to))
        {
            crownPiece(move.to);
        }
    }

    //add crown to piece
    public void crownPiece(Vector2Int piece)
    {
        GameObject newCrown = Instantiate(crown, getRealSpace(piece), Quaternion.identity);
        newCrown.transform.parent = pieces[piece].transform;
    }
}

public class GameState
{
    public bool isPlayersTurn;

    protected Dictionary<Vector2Int, PieceType> playerPieces = new Dictionary<Vector2Int, PieceType>();
    protected Dictionary<Vector2Int, PieceType> opponentPieces = new Dictionary<Vector2Int, PieceType>();

    //initialize game state
    public GameState(bool newIsPlayersTurn)
    {
        isPlayersTurn = newIsPlayersTurn;

        for(int y=0; y < 3; y++)
        {
            for(int x=y%2; x < 8; x+=2)
            {
                playerPieces.Add(new Vector2Int(x, y), PieceType.man);
            }
        }

        for(int y=5; y < 8; y++)
        {
            for(int x=y%2; x < 8; x+=2)
            {
                opponentPieces.Add(new Vector2Int(x, y), PieceType.man);
            }
        }
    }

    //generate new gamestate by applying move to old gamestate
    public GameState(GameState oldState, Move move)
    {
        playerPieces = new Dictionary<Vector2Int, PieceType>(oldState.playerPieces);
        opponentPieces = new Dictionary<Vector2Int, PieceType>(oldState.opponentPieces);

        if (oldState.isPlayersTurn)
        {
            playerPieces[move.to] = playerPieces[move.from];
            playerPieces.Remove(move.from);
            if (move.to.y == 7)
            {
                playerPieces[move.to] = PieceType.king;
            }
            foreach (Vector2Int jump in move.jumps)
            {
                opponentPieces.Remove(jump);
            }
        }
        else
        {
            opponentPieces[move.to] = opponentPieces[move.from];
            opponentPieces.Remove(move.from);
            if (move.to.y == 0)
            {
                opponentPieces[move.to] = PieceType.king;
            }
            foreach (Vector2Int jump in move.jumps)
            {
                playerPieces.Remove(jump);
            }
        }

        isPlayersTurn = !oldState.isPlayersTurn;
    }

    //return whether square contains a player's piece
    public bool isPlayer(Vector2Int square)
    {
        return playerPieces.ContainsKey(square);
    }

    //return whether square contains an opponent's piece
    public bool isOpponent(Vector2Int square)
    {
        return opponentPieces.ContainsKey(square);
    }

    //return whether square is empty
    public bool isEmpty(Vector2Int square)
    {
        return !playerPieces.ContainsKey(square) && !opponentPieces.ContainsKey(square);
    }

    //return whether square contains a king
    public bool isKing(Vector2Int square)
    {
        if (playerPieces.ContainsKey(square))
        {
            return playerPieces[square] == PieceType.king;
        }
        else if (opponentPieces.ContainsKey(square))
        {
            return opponentPieces[square] == PieceType.king;
        }
        return false;
    }

    //return number of pieces player has
    public int countPlayer()
    {
        return playerPieces.Count;
    }

    //return number of pieces opponent has
    public int countOpponent()
    {
        return opponentPieces.Count;
    }

    //return list of locations of the current players pieces
    public List<Vector2Int> getCurrentPlayerPieces()
    {
        if (isPlayersTurn)
        {
            return new List<Vector2Int>(playerPieces.Keys);
        }
        else
        {
            return new List<Vector2Int>(opponentPieces.Keys);
        }
    }
}

//defines a single move
public class Move
{
    public Vector2Int from;
    public Vector2Int to;
    public List<Vector2Int> jumps;

    public Move(Vector2Int newFrom, Vector2Int newTo)
    {
        from = newFrom;
        to = newTo;
        jumps = new List<Vector2Int>();
    }

    public Move(Vector2Int newFrom, Vector2Int newTo, List<Vector2Int> newJumps)
    {
        from = newFrom;
        to = newTo;
        jumps = newJumps;
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() != this.GetType())
        {    
            return false;
        }

        return Equals((Move)obj);
    }

    public bool Equals(Move otherMove)
    {
        if (otherMove is null)
        {
            if (this is null)
            {
                return true;
            }
            return false;
        }

        if (otherMove.from != this.from || otherMove.to != this.to)
        {
            return false;
        }

        if (otherMove.jumps.Count != this.jumps.Count){
            return false;
        }

        for(int i=0; i<otherMove.jumps.Count; i++)
        {
            if (otherMove.jumps.Count != this.jumps.Count)
            {
                return false;
            }
        }

        return true;
    }

    public static bool operator ==(Move lhs, Move rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(Move lhs, Move rhs)
    {
        return !lhs.Equals(rhs);
    }

    public override int GetHashCode()
    {
        return (from.ToString()+to.ToString()+string.Join(",",jumps)).GetHashCode();//not good
    }
}

//defines a node of a tree of possible future gamestates for all possible moves for minimaxing
public class Forecast
{
    public GameState gameState;

    public bool jumpPossible = false;
    public bool gameOver = false;
    public bool playerWins = false;

    protected Move move;
    protected int value;
    protected List<Forecast> forecasts = new List<Forecast>();

    public Forecast(GameState newGameState)
    {
        gameState = newGameState;
        //assign value of gamestate based on how many pieces each player has
        value = gameState.countOpponent() - gameState.countPlayer();
    }

    public Forecast(Move newMove, GameState oldGameState)
    {
        move = newMove;
        gameState = new GameState(oldGameState, move);
        //assign value of gamestate based on how many pieces each player has
        value = gameState.countOpponent() - gameState.countPlayer();
    }

    //generate a forecast tree of the passed depth recursively
    public void generate(int steps)
    {
        //if no children yet generated for this node of tree generate them
        if (forecasts.Count == 0)
        {
            string moveString = "";
            foreach (Move move in getPossibleMoves())
            {
                moveString+=move.from+">"+move.to+"("+string.Join(",",move.jumps)+")\n";
                forecasts.Add(new Forecast(move, gameState));
            }
            //if there were no possible moves after this gamestate it is a gameover state assign a high or low value depending on who wins
            if (forecasts.Count==0)
            {
                gameOver = true;
                if (gameState.isPlayersTurn)
                {
                    value = 100;
                }
                else
                {
                    value = -100;
                    playerWins = true;
                }
            }
            Debug.Log("Generate:\n"+moveString);
        }

        //if desired tree depth has not been reached call generate in each child
        if (steps > 1)
        {
            for (int i=0; i<forecasts.Count; i++)
            {
                forecasts[i].generate(steps - 1);
            }
        }
    }

    //get the child associated with the passed move
    public Forecast getForecast(Move move)
    {
        Debug.Log("Get:\n"+move.from+">"+move.to+"("+string.Join(",",move.jumps)+")\n");
        for(int i=0; i<forecasts.Count; i++)
        {
            if (forecasts[i].move == move)
            {
                return forecasts[i];
            }
        }

        return null;
    }

    //get all possible moves for a given gamestate
    List<Move> getPossibleMoves()
    {
        List<Vector2Int> currentPlayerPieces = gameState.getCurrentPlayerPieces();
        Vector2Int[] currentDirections;
        if (gameState.isPlayersTurn)
        {
            currentDirections = Board.playerDirections;
        }
        else
        {
            currentDirections = Board.opponentDirections;
        }

        //find all possible jumps
        List<Move> moves = new List<Move>();
        foreach (Vector2Int piece in currentPlayerPieces)
        {
            Vector2Int[] directions;
            if (gameState.isKing(piece))
            {
                directions = Board.kingDirections;
            }
            else
            {
                directions = currentDirections;
            }
            moves.AddRange(getJumps(piece, piece, new List<Vector2Int>(), directions));
        }

        //if a jump is found, mark that a jump is possible
        if (moves.Count > 0)
        {
            jumpPossible = true;
            return moves;
        }

        //if a jump is not found find all possible normal moves
        foreach (Vector2Int piece in currentPlayerPieces)
        {
            Vector2Int[] directions;
            if (gameState.isKing(piece))
            {
                directions = Board.kingDirections;
            }
            else if (gameState.isPlayer(piece))
            {
                directions = Board.playerDirections;
            }
            else
            {
                directions = Board.opponentDirections;
            }

            //find empty adjacent squares
            foreach (Vector2Int direction in directions)
            {
                Vector2Int from = piece;
                Vector2Int to = from + direction;
                List<Vector2Int> jumps = new List<Vector2Int>();
                if (Board.isOnBoard(to))
                {
                    if (gameState.isEmpty(to))
                    {
                        moves.Add(new Move(from, to, jumps));
                    }
                }
            }
        }

        return moves;
    }

    //find jumps possible from a given square recursively
    //(since multiple jumps can be made in one turn and there could be jump options at each stage)
    List<Move> getJumps(Vector2Int originalSquare, Vector2Int currentSquare, List<Vector2Int> jumps, Vector2Int[] directions)
    {
        List<Move> moves = new List<Move>();

        //find adjacent squares containing pieces from other side
        foreach (Vector2Int direction in directions)
        {
            Vector2Int to = currentSquare + direction;
            if (Board.isOnBoard(to))
            {
                if (!gameState.isEmpty(to) && gameState.isPlayer(to) != gameState.isPlayer(originalSquare) && !jumps.Contains(to))
                {
                    Vector2Int jump = to;
                    to = to + direction;

                    if (Board.isOnBoard(to))
                    {
                        //find if square on opposite side of piece is empty
                        if (gameState.isEmpty(to) || to == originalSquare)
                        {
                            //add jump to sequence of jumps
                            List<Vector2Int> newJumps = new List<Vector2Int>(jumps);
                            newJumps.Add(jump);
                            List<Move> multiJumps;

                            //call getJumps from this square to get all possible futher jumps, if there are none, return complete jump to possible moves
                            if ((to.y == 0 && gameState.isOpponent(originalSquare)) || (to.y == 7 && gameState.isPlayer(originalSquare)))
                            {
                                multiJumps = getJumps(originalSquare, to, newJumps, Board.kingDirections);
                            }
                            else
                            {
                                multiJumps = getJumps(originalSquare, to, newJumps, directions);
                            }

                            if (multiJumps.Count > 0){
                                moves.AddRange(multiJumps);
                            }
                            else
                            {
                                moves.Add(new Move(originalSquare, to, newJumps));
                            }
                        }
                    }
                }
            }
        }
        return moves;
    }

    //recursively evaluate the move tree using minimax
    public List<Move> evaluate(){
        //if have not reached end of tree
        if (forecasts.Count>0){
            if (gameState.isPlayersTurn)
            {
                value = 100;
            }
            else
            {
                value = -100;
            }
            List<Move> bestMoves = new List<Move>();
            for (int i=0; i<forecasts.Count; i++)
            {
                //evaluate children nodes
                forecasts[i].evaluate();
                //pick best children and add associated moves to list
                if (forecasts[i].value == value)
                {
                    bestMoves.Add(forecasts[i].move);
                }
                else if ((forecasts[i].value > value && !gameState.isPlayersTurn) || (forecasts[i].value < value && gameState.isPlayersTurn))
                {
                    bestMoves.Clear();
                    bestMoves.Add(forecasts[i].move);
                    value = forecasts[i].value;
                }
            }
            return bestMoves;
        }
        //return list of moves containing list of best moves (may be multiple equally good)
        return new List<Move>(){this.move};
    }
}

public enum side
{
    white,
    black
}

public enum PieceType
{
    man,
    king
}