using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PieceInteraction {
    public HumanTurn humanTurn {
        get; init;
    }

    public virtual void End() {
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

    public virtual void OnPieceMouseEnter(Piece piece) {
    }

    public virtual void OnPieceMouseExit(Piece piece) {
    }

    public virtual void OnPieceClicked(Piece piece) {
    }
}

// This is a dummy class so Visual Studio shuts up 
// https://stackoverflow.com/questions/64749385/predefined-type-system-runtime-compilerservices-isexternalinit-is-not-defined
namespace System.Runtime.CompilerServices {
    internal static class IsExternalInit {
    }
}