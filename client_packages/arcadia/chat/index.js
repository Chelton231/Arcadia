// Disables default RageMP Chat
mp.gui.chat.show(false);

// Initialize chatbox CEF, mark it as default server chat
const chatbox = mp.browsers.new('package://arcadia/chat/chat-ui/index.html');
chatbox.markAsChat();