/**
 * Studio Navigation Controller
 * Handles navigation within the Wurst Gaming studio sections
 * Expected result: Smooth section switching with proper active states
 */

class StudioNavigation {
    constructor() {
        this.currentSection = 'home';
        this.init();
    }
    
    init() {
        this.setupEventListeners();
        this.showSection('home'); // Default to home section
    }
    
    setupEventListeners() {
        // Handle navigation clicks
        document.addEventListener('click', (event) => {
            if (event.target.hasAttribute('onclick') && 
                event.target.getAttribute('onclick').includes('showStudioSection')) {
                event.preventDefault();
                
                // Extract section name from onclick attribute
                const match = event.target.getAttribute('onclick').match(/showStudioSection\('(.+?)'\)/);
                if (match) {
                    this.showSection(match[1]);
                }
            }
        });
    }
    
    /**
     * Show specific studio section
     * @param {string} sectionName - Name of section to show (home, studio)
     */
    showSection(sectionName) {
        // Hide all studio sections
        const sections = document.querySelectorAll('.studio-section');
        sections.forEach(section => {
            section.classList.remove('active');
            section.style.display = 'none';
        });
        
        // Show target section
        const targetSection = document.getElementById(`studio-${sectionName}`);
        if (targetSection) {
            targetSection.classList.add('active');
            targetSection.style.display = 'block';
        }
        
        // Update navigation active states
        this.updateNavigation(sectionName);
        
        this.currentSection = sectionName;
        
        console.log(`Studio section switched to: ${sectionName}`);
    }
    
    /**
     * Update navigation link active states
     * @param {string} activeSectionName - Name of currently active section
     */
    updateNavigation(activeSectionName) {
        // Remove active class from all studio nav links
        const navLinks = document.querySelectorAll('#studio-site nav a');
        navLinks.forEach(link => {
            link.classList.remove('active');
        });
        
        // Add active class to current section link
        const activeLink = document.querySelector(`#studio-site nav a[onclick*="${activeSectionName}"]`);
        if (activeLink) {
            activeLink.classList.add('active');
        } else if (activeSectionName === 'home') {
            // Home link doesn't have showStudioSection call, so find it differently
            const homeLink = document.querySelector('#studio-site nav a[onclick*="home"]') || 
                            document.querySelector('#studio-site nav a:first-child');
            if (homeLink) {
                homeLink.classList.add('active');
            }
        }
    }
    
    /**
     * Get current active section
     * @returns {string} Current section name
     */
    getCurrentSection() {
        return this.currentSection;
    }
}

// Global function for HTML event handlers
function showStudioSection(sectionName) {
    if (window.studioNavigation) {
        window.studioNavigation.showSection(sectionName);
    }
}

// Initialize studio navigation when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    window.studioNavigation = new StudioNavigation();
});