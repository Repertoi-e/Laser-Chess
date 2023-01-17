using System.Collections;

// We are handling things with transactions because a real game
// will probably have a system like that in place (or something along those lines)
// to support stuff like undo/redo, and replays. Transactions are stuff that happens in 
// the game on the board: piece selection/moving, attacking, etc. (both for humans and AI).
// Besides serialization, using "transactions" helps keep the code orthogonal -
// different parts of the game can queue them up, and all are executed at the end of
// the game loop. The bad alternative would be if every part of the game could execute
// potentially arbitrary many actions, so everything becomes coupled and spaghetti-like.
public abstract class Transaction {
    public abstract bool IsValid();
    public abstract IEnumerator Execute();

}
