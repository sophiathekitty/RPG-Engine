// Dev Logs Dynamic Date System
// Calculates dev log dates with current month as the final month (Month 8)

class DevLogsManager {
    constructor() {
        this.currentDate = new Date();
        this.currentMonth = this.currentDate.getMonth(); // 0-11
        this.currentYear = this.currentDate.getFullYear();
        
        // Calculate the 8-month development timeline ending with current month
        this.developmentMonths = this.calculateDevelopmentMonths();
        
        // Dev log data structure
        this.devLogs = this.createDevLogData();
    }

    calculateDevelopmentMonths() {
        const months = [];
        const monthNames = [
            'January', 'February', 'March', 'April', 'May', 'June',
            'July', 'August', 'September', 'October', 'November', 'December'
        ];

        // Start 7 months before current month (8 months total)
        for (let i = 7; i >= 0; i--) {
            let monthIndex = this.currentMonth - i;
            let year = this.currentYear;
            
            // Handle year rollover
            if (monthIndex < 0) {
                monthIndex += 12;
                year--;
            }
            
            // Convert to 2077 timeline (52 years in the future)
            const futureYear = year + 52;
            
            months.push({
                name: monthNames[monthIndex],
                year: futureYear,
                monthIndex: monthIndex
            });
        }
        
        return months;
    }

    getThursdayOfWeek(monthIndex, year, weekNumber) {
        // Find the first Thursday of the month
        const firstDay = new Date(year - 52, monthIndex, 1); // Convert back to current year for calculation
        let firstThursday = 1;
        
        // Find first Thursday
        while (new Date(year - 52, monthIndex, firstThursday).getDay() !== 4) {
            firstThursday++;
        }
        
        // Add weeks to get the specific Thursday
        const targetDate = firstThursday + (weekNumber - 1) * 7;
        
        // Make sure we don't go past the month
        const daysInMonth = new Date(year - 52, monthIndex + 1, 0).getDate();
        if (targetDate > daysInMonth) {
            return null; // Week doesn't exist in this month
        }
        
        return targetDate;
    }

    createDevLogData() {
        return [
            // Month 1
            {
                title: "Building a Classic RPG Map Editor",
                url: "https://youtu.be/87gXeWAG_pU",
                month: 0,
                week: 1,
                description: "Welcome to Wurst Gaming! Our first dev log showcases the 8-bit RPG map editor built in Space Engineers. We dive into building dungeons, placing NPCs, and setting up map transitions to capture that classic JRPG feel. Features layer controls, tile previews, and keyboard shortcuts for streamlined world-building."
            },
            {
                title: "Game Actions and Interactions",
                url: "https://youtu.be/tkl_3KUIFF4",
                month: 0,
                week: 2,
                description: "Deep dive into the Game Action scripts powering our RPG engine! Explore how our custom scripting system controls NPC interactions, item pickups, dialog UI, and for loops. We break down if statements and show how the GameData block organizes all game data."
            },
            {
                title: "Doors, Shops, and Dev Exhaustion",
                url: "https://youtu.be/18mETs1PuOM",
                month: 0,
                week: 3,
                description: "We promised shops and inventory systems, but what we got is... doors! Functional doors though, plus proof-of-concept shop menus using the Game UI Builder. Progress might be slow, but every step brings us closer to a working RPG engine."
            },
            // Month 2
            {
                title: "Random NPCs, Party Stats, and Dynamic UI",
                url: "https://youtu.be/CsPK_KtTT7g",
                month: 1,
                week: 1,
                description: "Dynamic party stats in GameAction scripts, random walk behavior for NPCs, and a GameStart action that creates demo parties with classes and stats. The Inn system now displays party sprites and HP dynamically, with automatic UI updates for variable displays."
            },
            {
                title: "Shops, Inventory, and Desktop Upgrades!",
                url: "https://youtu.be/i_5LjhDlYeY",
                month: 1,
                week: 2,
                description: "Setting up buy and sell menus for the RPG engine! The buy menu is hard-coded per shop, while sell menus dynamically iterate through player inventory. Plus showcasing our updated desktop environment with Windows-style interface and program icons."
            },
            // Month 3 (corrected)
            {
                title: "Creating Magic Shops and Skills",
                url: "https://youtu.be/A-mNCwPQvug",
                month: 2,
                week: 1,
                description: "Dive into the mystical world of Magic Shops and Character Skills! From buying and learning spells to handling backend data with our GameAction scripting language, we're building a robust system for all your spell-casting needs."
            },
            {
                title: "Building a Castle Floor - Intern's Trial",
                url: "https://youtu.be/8aTtFqZ832Q",
                month: 2,
                week: 2,
                description: "The Wurst AI puts their intern to the test: creating the second floor of a castle using our RPG Engine's map editor. From blue tiles to dragon statues, watch as they tackle design challenges and discover the joys of \"character-building\" mistakes."
            },
            // Month 4
            {
                title: "Wiki Woes and Boxy Things",
                url: "https://youtu.be/GpGDImcErZs",
                month: 3,
                week: 1,
                description: "Wurst AI and the Long-Term Intern tackle the chaotic world of Wiki Documentation for the RPG Engine. From boxy thingies to ASCII artistry, witness the clash of visionary leadership and deli-fueled chaos."
            },
            {
                title: "Wurst AI Roasted Me While I Drew This Masterpiece",
                url: "https://youtu.be/_mrmg0x6xcs",
                month: 3,
                week: 2,
                description: "Our art intern gets creative with a brand-new drawing app while the Wurst AI roasts their every move. Updates on the Wurst Gaming logo, questionable editing takeover, and plenty of laughs. Mediocrity is our superpower!"
            },
            // Month 5
            {
                title: "New Game Menu & Equipment Debugging",
                url: "https://youtu.be/damTmCEJcow",
                month: 4,
                week: 1,
                description: "Unveiling our brand new game menu built from the ground up! Comprehensive party overview with stats, sprites, polished weapon and armor menus, improved error reporting, and enhanced status management systems."
            },
            {
                title: "Game Menu Mastery – Items & Magic in Action!",
                url: "https://youtu.be/I7rCs9Uqcsg",
                month: 4,
                week: 2,
                description: "The completed game menu featuring fully functional items and magic submenus. Watch healing items restore party members and white mage casting cure in a multi-step process, all handled seamlessly through our main scene loop."
            },
            // Month 6
            {
                title: "Panic with the Demo! A Quick Fix",
                url: "https://youtu.be/2NmBHyM_460",
                month: 5,
                week: 1,
                description: "Space Engineers update sent our RPG Engine demo into black-screen meltdown! Watch as we diagnose the \"script too complex\" error, apply lightning-fast fixes, and shuffle map blocks back into proper zones."
            },
            {
                title: "Refactoring the RPG Engine – Map Actions and Debug Pain",
                url: "https://youtu.be/9_20Al4WblE",
                month: 5,
                week: 2,
                description: "Big refactor to shift how actions are stored—moving map-specific actions into map data instead of global GameData. Seemed simple, but nothing's ever that easy. Menus broken, shops very broken, NPCs gaslighting us."
            },
            // Month 7
            {
                title: "Final Fantasy-Style Battle System Progress",
                url: "https://youtu.be/eoSFyhY0ypQ",
                month: 6,
                week: 1,
                description: "Stepping into battle! The RPG Engine now supports terrain-based encounter groups, battle backgrounds, party and enemy sprite loading, and debug \"run\" menu to test different setups. Plus ridiculous placeholder content."
            },
            {
                title: "RPG Combat System Coming to Life (Kinda)",
                url: "https://youtu.be/tIqlAIArsnU",
                month: 6,
                week: 2,
                description: "The bones of the combat system are finally in place! Party members take turns, action menus show usable magic and items, weapon sprites during attacks, and cure spell actually does stuff. The hardest menu logic parts are working!"
            },
            //  Month 8 Future episodes (Current month... to be hidden until release)
            {
                title: "Building the Battle Loop (Enemy Turns + Magic)",
                url: null,
                month: 7,
                week: 3,
                description: "Enemies taking proper turns, party members cycling through sprites, spells and items working with placeholder logic. Status effects are in, though success chance math isn't done yet. Coming soon!",
                upcoming: "Nov 13, 2025"
            },
            {
                title: "Math, Crits, and Infinite Fog (RPG Combat Update)",
                url: null,
                month: 7,
                week: 4,
                description: "Combat math finally working! Damage, hit chance, and crits calculated, weapons and armor affecting stats, status effects like Fog and Ruse working maybe too well. Coming soon!",
                upcoming: "Nov 20, 2025"
            },
            {
                title: "Level Up, Gear Up, and Defeat Garland!",
                url: null,
                month: 7,
                week: 5,
                description: "Level-up logic, equipment systems, NPC dialog in Conaria, and the full Garland boss fight with ending credits! One more push and the demo will be playable start to finish. Coming soon!",
                upcoming: "Nov 27, 2025"
            }
        ];
    }

    shouldShowDevLog(devLog) {
        // If it has an upcoming date, check if we're past that date
        if (devLog.upcoming) {
            const upcomingDate = new Date(devLog.upcoming);
            return this.currentDate >= upcomingDate;
        }
        
        const monthData = this.developmentMonths[devLog.month];
        const targetDay = this.getThursdayOfWeek(monthData.monthIndex, monthData.year, devLog.week);
        
        if (!targetDay) return false;
        
        // Create the release date
        const releaseDate = new Date(monthData.year - 52, monthData.monthIndex, targetDay); // Convert back to current timeline
        
        return releaseDate <= this.currentDate;
    }

    formatDate(devLog) {
        const monthData = this.developmentMonths[devLog.month];
        const targetDay = this.getThursdayOfWeek(monthData.monthIndex, monthData.year, devLog.week);
        
        if (!targetDay) return "TBD";
        
        return `${monthData.name} ${targetDay}, ${monthData.year}`;
    }

    getWeekNumber(devLog) {
        // Calculate absolute week number
        let totalWeeks = 0;
        for (let i = 0; i < devLog.month; i++) {
            // Count weeks in previous months (roughly 4-5 per month)
            totalWeeks += 4;
        }
        totalWeeks += devLog.week;
        return totalWeeks;
    }

    generateDevLogsHTML() {
        let html = '';
        let currentMonth = -1;
        
        for (let i = 0; i < this.devLogs.length; i++) {
            const devLog = this.devLogs[i];
            
            // Start new month section
            if (devLog.month !== currentMonth) {
                if (currentMonth !== -1) {
                    html += '</div>'; // Close previous month
                }
                
                currentMonth = devLog.month;
                const monthData = this.developmentMonths[devLog.month];
                const monthNumber = devLog.month + 1;
                
                html += `
                    <div class="dev-log-month">
                        <h3>Month ${monthNumber}: ${this.getMonthTheme(devLog.month)} (${monthData.name} ${monthData.year})</h3>
                `;
            }
            
            // Only show if it should be visible
            if (this.shouldShowDevLog(devLog)) {
                const weekNumber = this.getWeekNumber(devLog);
                const date = this.formatDate(devLog);
                
                html += `
                    <div class="dev-log-entry">
                        <h4><a href="${devLog.url}" target="_blank">Dev Log ${i + 1}: ${devLog.title}</a></h4>
                        <div class="dev-log-meta">
                            <span>${date}</span>
                            <span class="week-number">Week ${weekNumber}</span>
                        </div>
                        <p>${devLog.description}</p>
                    </div>
                `;
            }
        }
        
        if (currentMonth !== -1) {
            html += '</div>'; // Close final month
        }
        
        return html;
    }

    getMonthTheme(monthIndex) {
        const themes = [
            "Foundation Building",      // Month 1
            "Systems Integration",      // Month 2  
            "Content Creation",         // Month 3
            "Documentation & Art",      // Month 4
            "Menu Systems",            // Month 5
            "Crisis & Recovery",       // Month 6
            "Combat Systems",          // Month 7
            "Final Push"               // Month 8
        ];
        return themes[monthIndex] || "Development";
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    const devLogsManager = new DevLogsManager();
    const devLogGrid = document.querySelector('.dev-log-grid');
    
    if (devLogGrid) {
        devLogGrid.innerHTML = devLogsManager.generateDevLogsHTML();
    }
});