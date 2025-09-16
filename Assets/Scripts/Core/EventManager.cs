using UnityEngine.Events;

public static class EventManager
{
    // Invocado quando o jogador acerta uma combinação
    // Parâmetros: (DraggableItem item, DropSlot slot)
    public static UnityAction<DraggableItem, DropSlot> OnCorrectMatch;

    // Invocado quando o jogador erra uma combinação
    // Parâmetros: (DraggableItem item)
    public static UnityAction<DraggableItem> OnIncorrectMatch;
    
    // Invocado quando a cena/rodada é completada
    public static UnityAction OnRoundCompleted;

    // Invocado quando o jogo termina
    public static UnityAction OnGameEnd;
    
    // Invocado para atualizar a UI de pontuação
    public static UnityAction<int> OnScoreUpdated;

    // Invocado para atualizar a UI do tempo
    public static UnityAction<float> OnTimeUpdated;
}