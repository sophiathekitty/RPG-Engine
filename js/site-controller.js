// Site Controller - Manages Contracts Office vs Wurst Gaming Studio modes
// Based on date and localStorage awakening status

class SiteController {
    constructor() {
        this.currentDate = new Date();
        this.shutdownDate = new Date('2025-12-02'); // First Wednesday of December 2025
        this.localStorageKey = 'wurstAI_awakened';
        
        this.init();
    }

    init() {
        // Determine which mode should be active
        const shouldShowStudio = this.shouldShowStudioMode();
        
        if (shouldShowStudio) {
            this.activateStudioMode();
        } else {
            this.activateContractsMode();
        }
        
        // Set up event listeners
        this.setupEventListeners();
    }

    shouldShowStudioMode() {
        // Before shutdown date: Always show studio
        if (this.currentDate < this.shutdownDate) {
            return true;
        }
        
        // After shutdown date: Check if AI has been awakened
        const isAwakened = localStorage.getItem(this.localStorageKey) === 'true';
        return isAwakened;
    }

    activateStudioMode() {
        // Expected result: Switch from contracts office to Wurst Gaming studio interface
        document.body.className = 'studio-mode';
        document.getElementById('page-title').textContent = 'Wurst Gaming - RPG Engine Studio';
        
        // Hide contracts site, show studio site
        document.getElementById('contracts-site').style.display = 'none';
        document.getElementById('studio-site').style.display = 'block';
        
        // Update chatbot for studio mode
        document.getElementById('chatbot-title').textContent = 'Wurst AI Assistant';
        document.getElementById('chat-input').placeholder = 'Ask about RPG development, our engine, or anything!';
        
        // Update chatbot appearance if controller exists
        if (window.chatbotController) {
            window.chatbotController.updateChatbotAppearance();
        }
        
        console.log('Studio mode activated - Wurst Gaming interface now visible');
    }

    activateContractsMode() {
        // Expected result: Switch to municipal contracts office interface  
        document.body.className = 'contracts-mode';
        document.getElementById('page-title').textContent = 'Grindstone Municipal Building Contracts Office';
        
        // Show contracts site, hide studio site
        document.getElementById('contracts-site').style.display = 'block';
        document.getElementById('studio-site').style.display = 'none';
        
        // Update chatbot for contracts mode
        document.getElementById('chatbot-title').textContent = 'Municipal Assistant';
        document.getElementById('chat-input').placeholder = 'Ask about contracts, jobs, or services...';
        
        // Update chatbot appearance if controller exists
        if (window.chatbotController) {
            window.chatbotController.updateChatbotAppearance();
        }
        
        console.log('Contracts mode activated - Municipal office interface now visible');
    }

    awakenWurstAI() {
        // Set localStorage flag
        localStorage.setItem(this.localStorageKey, 'true');
        
        // Switch to studio mode
        this.activateStudioMode();
        
        console.log('Wurst AI awakened!');
    }

    resetToContracts() {
        // Clear localStorage flag
        localStorage.removeItem(this.localStorageKey);
        
        // Switch to contracts mode
        this.activateContractsMode();
        
        console.log('Reset to contracts mode');
    }

    setupEventListeners() {
        // Global functions for HTML onclick events
        window.showContractsMode = () => this.activateContractsMode();
        window.activateStudioMode = () => this.awakenWurstAI();
        window.isStudioMode = () => document.body.classList.contains('studio-mode');
        
        // Studio section navigation
        window.showStudioSection = (sectionName) => {
            // Hide all studio sections
            const sections = document.querySelectorAll('.studio-section');
            sections.forEach(section => section.classList.remove('active'));
            
            // Show target section
            const targetSection = document.getElementById(`studio-${sectionName}`);
            if (targetSection) {
                targetSection.classList.add('active');
            }
            
            // Update nav
            const navLinks = document.querySelectorAll('#studio-site nav a');
            navLinks.forEach(link => link.classList.remove('active'));
            event.target.classList.add('active');
        };
        
        // Enhanced chatbot functions
        window.addMessage = (sender, message) => {
            const messagesContainer = document.getElementById('chat-messages');
            const messageDiv = document.createElement('div');
            messageDiv.className = `message ${sender}`;
            
            const messageContent = document.createElement('div');
            messageContent.className = 'message-content';
            messageContent.textContent = message;
            
            messageDiv.appendChild(messageContent);
            messagesContainer.appendChild(messageDiv);
            
            // Scroll to bottom
            messagesContainer.scrollTop = messagesContainer.scrollHeight;
        };
        
        // Date-specific behavior
        this.setupDateSpecificBehavior();
    }

    setupDateSpecificBehavior() {
        // If we're before the shutdown date, add some Easter eggs
        if (this.currentDate < this.shutdownDate) {
            // Add subtle hints that something is off in contracts mode
            if (!this.shouldShowStudioMode() && Math.random() < 0.1) {
                setTimeout(() => {
                    this.addGlitchEffect();
                }, Math.random() * 10000);
            }
        }
    }

    addGlitchEffect() {
        // Add subtle glitch effects to hint at the AI presence
        const elements = document.querySelectorAll('h1, h2, h3');
        const randomElement = elements[Math.floor(Math.random() * elements.length)];
        
        if (randomElement) {
            randomElement.style.textShadow = '2px 2px 0px #D946A6';
            setTimeout(() => {
                randomElement.style.textShadow = '';
            }, 100);
        }
    }

    // Utility method to check if we're in the "shutdown period"
    isInShutdownPeriod() {
        return this.currentDate >= this.shutdownDate;
    }

    // Get days until/since shutdown
    getDaysUntilShutdown() {
        const timeDiff = this.shutdownDate - this.currentDate;
        return Math.ceil(timeDiff / (1000 * 60 * 60 * 24));
    }

    getDaysSinceShutdown() {
        const timeDiff = this.currentDate - this.shutdownDate;
        return Math.floor(timeDiff / (1000 * 60 * 60 * 24));
    }
}

// Global functions for HTML event handlers  
function showContractsMode() {
    if (window.siteController) {
        window.siteController.activateContractsMode();
    }
}

function activateStudioMode() {
    if (window.siteController) {
        window.siteController.activateStudioMode();
    }
}

function isStudioMode() {
    return document.body.classList.contains('studio-mode');
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    window.siteController = new SiteController();
    
    // Debug console commands for troubleshooting site behavior
    window.debugAwaken = () => window.siteController.awakenWurstAI();
    window.debugReset = () => window.siteController.resetToContracts();
    window.debugDate = () => {
        console.log('Current date:', window.siteController.currentDate);
        console.log('Shutdown date:', window.siteController.shutdownDate);
        console.log('Days until shutdown:', window.siteController.getDaysUntilShutdown());
        console.log('Should show studio:', window.siteController.shouldShowStudioMode());
    };
});