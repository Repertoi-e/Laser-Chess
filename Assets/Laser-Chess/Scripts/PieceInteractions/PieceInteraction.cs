public abstract class PieceInteraction {
    protected PlayingState playingState;
    protected HumanTurn humanTurn;

    public Piece piece;

    public PieceInteraction(Piece piece) {
        this.piece = piece;
        playingState = GameState.CurrentState as PlayingState;
        humanTurn = playingState?.Turn as HumanTurn;
    }

    public virtual bool IsAvailable() {
        return true;
    }

    public virtual void Cleanup() {
    }

    public virtual void OnBoardMouseEnter() {
    }

    public virtual void OnBoardMouseExit() {
    }

    public virtual void OnTileMouseEnter(Tile tile) {
    }

    public virtual void OnTileMouseExit(Tile tile) {
    }

    public virtual void OnTileClicked(Tile tile) {
    }

    public virtual void OnPieceMouseEnter(Piece target) {
    }

    public virtual void OnPieceMouseExit(Piece target) {
    }

    public virtual void OnPieceClicked(Piece target) {
    }
}