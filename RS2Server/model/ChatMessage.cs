using RS2.Server.player;

namespace RS2.Server.model
{
    internal class ChatMessage
    {
        private int colour;
        private string chatText;
        private Player player;
        private int effect;
        private byte[] packed;

        public ChatMessage(int colour, string chatText, int effect, Player p, byte[] packed)
        {
            this.colour = colour;
            this.chatText = chatText;
            this.effect = effect;
            this.player = p;
            this.packed = packed;
        }

        public int getColour()
        {
            return colour;
        }

        public string getChatText()
        {
            return chatText;
        }

        public Player getPlayer()
        {
            return player;
        }

        public int getEffect()
        {
            return effect;
        }

        public byte[] getPacked()
        {
            return packed;
        }

        public byte getPacked(int i)
        {
            return packed[i];
        }
    }
}