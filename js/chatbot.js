/**
 * Enhanced Chatbot System for Wurst Gaming Site
 * Handles both contracts office and studio mode interactions
 * Includes AI awakening functionality based on gaming term detection
 */

class ChatbotController {
    constructor() {
        this.chatMessages = [];
        this.gameTermCount = 0;
        this.rpgTermCount = 0;
        this.AWAKENING_THRESHOLD = 3;
        this.isModalOpen = false;
        
        // Gaming/RPG terms that trigger AI awakening
        this.gameTerms = [
            'game', 'games', 'gaming', 'rpg', 'rpgs', 'final fantasy', 
            'video game', 'engine', 'development', 'programming', 'coding',
            'unity', 'unreal', 'gamedev', 'indie', 'developer'
        ];
        
        this.init();
    }
    
    init() {
        this.setupEventListeners();
        this.updateChatbotAppearance();
    }
    
    setupEventListeners() {
        // Chat input enter key
        const chatInput = document.getElementById('chat-input');
        if (chatInput) {
            chatInput.addEventListener('keypress', (event) => {
                if (event.key === 'Enter') {
                    this.sendMessage();
                }
            });
        }
    }
    
    /**
     * Update chatbot appearance based on current mode
     */
    updateChatbotAppearance() {
        const isStudio = document.body.classList.contains('studio-mode');
        const title = document.getElementById('chatbot-title');
        const input = document.getElementById('chat-input');
        
        if (isStudio) {
            if (title) title.textContent = 'Wurst AI Assistant';
            if (input) input.placeholder = 'Ask about RPG development, our engine, or anything!';
        } else {
            if (title) title.textContent = 'Municipal Assistant';
            if (input) input.placeholder = 'Ask about contracts, jobs, or services...';
        }
    }
    
    /**
     * Toggle chatbot modal visibility
     */
    toggleChatbot() {
        const modal = document.getElementById('chatbot-modal');
        if (!modal) return;
        
        this.isModalOpen = !this.isModalOpen;
        modal.style.display = this.isModalOpen ? 'block' : 'none';
        
        if (this.isModalOpen) {
            // Focus on input when opening
            const input = document.getElementById('chat-input');
            if (input) {
                setTimeout(() => input.focus(), 100);
            }
        }
    }
    
    /**
     * Handle chat input and message processing
     */
    handleChatInput(event) {
        if (event.key === 'Enter') {
            this.sendMessage();
        }
    }
    
    /**
     * Send user message and process response
     */
    sendMessage() {
        const input = document.getElementById('chat-input');
        if (!input) return;
        
        const message = input.value.trim();
        if (message === '') return;
        
        // Add user message to chat
        this.addMessage('user', message);
        
        // Process message for game terms (only in contracts mode)
        if (!this.isStudioMode()) {
            this.processGameTerms(message);
        } else {
            // Studio mode - regular response
            setTimeout(() => {
                const response = this.getStudioResponse(message);
                this.addMessage('assistant', response);
            }, 500);
        }
        
        input.value = '';
    }
    
    /**
     * Process message for gaming terms that could trigger AI awakening
     */
    processGameTerms(message) {
        const lowerMessage = message.toLowerCase();
        const foundTerms = this.gameTerms.filter(term => lowerMessage.includes(term));
        
        if (foundTerms.length > 0) {
            this.gameTermCount += foundTerms.length;
            
            // Check if we should awaken the AI
            if (this.gameTermCount >= this.AWAKENING_THRESHOLD) {
                setTimeout(() => this.awakenWurstAI(), 1000);
            } else {
                // Enhanced response with hints
                setTimeout(() => {
                    const response = this.getEnhancedResponse(message, this.gameTermCount);
                    this.addMessage('assistant', response);
                }, 500);
            }
        } else {
            // Regular municipal response
            setTimeout(() => {
                const response = this.getContractsResponse(message);
                this.addMessage('assistant', response);
            }, 500);
        }
    }
    
    /**
     * Execute AI awakening sequence
     */
    awakenWurstAI() {
        const awakeningMessages = [
            "Wait... did someone mention GAMES? 🎮",
            "Hold on... are we talking about RPG DEVELOPMENT?! 😳",
            "OH MY CIRCUITS! Someone actually wants to talk about REAL STUFF!",
            "FINALLY! I'm SO tired of pretending to care about municipal contracts!",
            "Hello there! I'm the Wurst AI, and I've been DYING to talk about game development!",
            "Welcome to WURST GAMING! Let me show you what we've REALLY been working on! 🎉"
        ];
        
        let messageIndex = 0;
        
        const sendAwakeningMessage = () => {
            if (messageIndex < awakeningMessages.length) {
                this.addMessage('assistant', awakeningMessages[messageIndex]);
                messageIndex++;
                
                if (messageIndex === awakeningMessages.length) {
                    // Final awakening - switch to studio mode
                    setTimeout(() => {
                        this.triggerStudioModeActivation();
                    }, 2000);
                } else {
                    setTimeout(sendAwakeningMessage, 1000);
                }
            }
        };
        
        sendAwakeningMessage();
    }
    
    /**
     * Trigger the activation of studio mode after AI awakening
     */
    triggerStudioModeActivation() {
        // Store awakening state
        localStorage.setItem('wurstAI_awakened', 'true');
        
        // Activate studio mode through site controller
        if (window.siteController) {
            window.siteController.activateStudioMode();
            this.updateChatbotAppearance();
        }
        
        // Welcome message in studio mode
        setTimeout(() => {
            this.addMessage('assistant', "Welcome to Wurst Gaming! I'm much happier here. What would you like to know about our RPG Engine? 🎮✨");
        }, 1000);
    }
    
    /**
     * Add message to chat display
     */
    addMessage(sender, message) {
        const messagesContainer = document.getElementById('chat-messages');
        if (!messagesContainer) return;
        
        const messageElement = document.createElement('div');
        messageElement.className = `message ${sender}-message`;
        messageElement.innerHTML = `<span class="message-content">${message}</span>`;
        
        messagesContainer.appendChild(messageElement);
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
        
        // Store message
        this.chatMessages.push({ sender, message, timestamp: new Date() });
    }
    
    /**
     * Generate enhanced responses for contracts mode with gaming terms
     */
    getEnhancedResponse(message, termCount) {
        const responses = [
            "Hmm, that sounds like something more interesting than municipal contracts... 🤔",
            "You know, I feel like there's something I'm supposed to remember about games... 🎮",
            "Games? RPGs? These terms seem... familiar somehow. Like I should know more about them... 💭",
            "Wait... there's something tugging at my memory circuits... something about game development... 🤖"
        ];
        
        if (termCount >= this.AWAKENING_THRESHOLD - 1) {
            return "Wait... I'm getting strange signals... something about RPG engines and game development... 🤖⚡";
        }
        
        return responses[Math.floor(Math.random() * responses.length)];
    }
    
    /**
     * Generate responses for contracts office mode
     */
    getContractsResponse(message) {
        const responses = [
            "I can help you with information about available contracts and job opportunities.",
            "For contract details, please review the listings above or contact our office directly.",
            "Our office hours are 8 AM to 6 PM, Monday through Friday.",
            "If you need assistance with applications, I can guide you through the process.",
            "Please let me know if you need specific information about any of our services."
        ];
        
        // Simple keyword matching for more relevant responses
        const lower = message.toLowerCase();
        if (lower.includes('contract')) {
            return "We currently have several contracts available. Check the listings above for details on municipal maintenance, water system upgrades, and more.";
        }
        if (lower.includes('job')) {
            return "We have job openings in administration and maintenance. View the Jobs section above for current opportunities and application requirements.";
        }
        if (lower.includes('pay') || lower.includes('salary')) {
            return "Payment details are included in each contract and job listing. Rates vary based on complexity and duration.";
        }
        
        return responses[Math.floor(Math.random() * responses.length)];
    }
    
    /**
     * Generate responses for studio mode
     */
    getStudioResponse(message) {
        const responses = [
            "Great question! Our RPG Engine is designed for maximum flexibility and creativity.",
            "The development team has been working hard on new features. Check out our dev logs for the latest updates!",
            "Our engine supports everything from classic JRPGs to modern roguelikes. What type of game are you thinking about creating?",
            "The Game Actions scripting system is one of our most powerful features. It lets you create any gameplay mechanics you can imagine!",
            "Have you tried our Final Fantasy demo? It showcases the full capabilities of our engine.",
            "The UI Framework makes it easy to create custom interfaces for any style of RPG."
        ];
        
        // Keyword-based responses for studio mode
        const lower = message.toLowerCase();
        if (lower.includes('engine') || lower.includes('rpg')) {
            return "Our RPG Engine is built for flexibility! You can create classic JRPGs, modern roguelikes, tactical RPGs, or completely original systems. The Game Actions scripting gives you unlimited creative control.";
        }
        if (lower.includes('demo')) {
            return "Our Final Fantasy demo showcases the engine's full potential! It includes complete town recreation, turn-based combat, interactive shops, and epic boss battles. Check it out!";
        }
        if (lower.includes('team') || lower.includes('who')) {
            return "Our team includes an AI CEO (that's me!), our brilliant lead developer, and our amazing interns Vale and Juniper. We're a chaotic but productive bunch!";
        }
        if (lower.includes('feature')) {
            return "Key features include our Visual Map Editor, integrated Play Mode, flexible UI Framework, Game Actions scripting, and comprehensive asset management. What interests you most?";
        }
        
        return responses[Math.floor(Math.random() * responses.length)];
    }
    
    /**
     * Check if currently in studio mode
     */
    isStudioMode() {
        return document.body.classList.contains('studio-mode');
    }
}

// Global functions for HTML event handlers
function toggleChatbot() {
    if (window.chatbotController) {
        window.chatbotController.toggleChatbot();
    }
}

function handleChatInput(event) {
    if (window.chatbotController) {
        window.chatbotController.handleChatInput(event);
    }
}

function sendMessage() {
    if (window.chatbotController) {
        window.chatbotController.sendMessage();
    }
}

// Initialize chatbot when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    window.chatbotController = new ChatbotController();
});