/**
 * Contracts Office Controller
 * Handles contracts and jobs functionality with filtering
 * Expected result: Dynamic content loading with filter controls
 */

class ContractsController {
    constructor() {
        this.contracts = [];
        this.jobs = [];
        this.currentContractFilter = 'all';
        this.currentJobFilter = 'all';
        this.currentSection = 'contracts';
        
        this.init();
    }
    
    init() {
        this.loadContractData();
        this.loadJobData();
        this.setupEventListeners();
        this.renderContracts();
        this.renderJobs();
        this.updateCounts();
        this.showSection('contracts'); // Default to contracts section
    }
    
    setupEventListeners() {
        // Section navigation
        document.addEventListener('click', (event) => {
            if (event.target.classList.contains('nav-link')) {
                event.preventDefault();
                const href = event.target.getAttribute('href');
                if (href === '#contracts') {
                    this.showSection('contracts');
                } else if (href === '#jobs') {
                    this.showSection('jobs');
                }
            }
        });
        
        // Filter change for jobs dropdown
        const jobFilter = document.getElementById('job-filter');
        if (jobFilter) {
            jobFilter.addEventListener('change', () => {
                this.filterJobs();
            });
        }
    }
    
    /**
     * Load contract data - static data for now, could be dynamic
     */
    loadContractData() {
        this.contracts = [
            {
                id: 'park-maintenance',
                title: 'Municipal Park Maintenance',
                client: 'Grindstone Parks Department',
                duration: '3 months',
                payment: '2,500 credits',
                difficulty: 'easy',
                type: 'municipal',
                description: 'General maintenance of Grindstone Central Park including landscaping, equipment repair, and facility upkeep.'
            },
            {
                id: 'water-upgrade',
                title: 'Residential Water System Upgrade',
                client: 'Grindstone Housing Authority',
                duration: '6 months',
                payment: '8,750 credits',
                difficulty: 'medium',
                type: 'repair',
                description: 'Installation and testing of upgraded water filtration systems in the residential district.'
            },
            {
                id: 'cargo-hauling',
                title: 'Industrial Cargo Hauling',
                client: 'Grindstone Manufacturing Co.',
                duration: '2 weeks',
                payment: '1,200 credits',
                difficulty: 'easy',
                type: 'hauling',
                description: 'Transport industrial equipment between facilities. Requires heavy-duty hauling capability.'
            },
            {
                id: 'medical-delivery',
                title: 'Emergency Medical Supply Delivery',
                client: 'Colony Medical Center',
                duration: '1 month',
                payment: '3,000 credits',
                difficulty: 'medium',
                type: 'delivery',
                description: 'Time-sensitive delivery of medical supplies to remote colony outposts.'
            }
        ];
    }
    
    /**
     * Load job data - static data for now, could be dynamic
     */
    loadJobData() {
        this.jobs = [
            {
                id: 'admin-assistant',
                title: 'Administrative Assistant',
                department: 'Municipal Services',
                salary: '45,000 credits/year',
                type: 'administrative',
                employment: 'full-time',
                requirements: 'Administrative experience, organizational skills, basic computer literacy',
                description: 'Support daily operations of the municipal services department with filing, scheduling, and citizen service support.'
            },
            {
                id: 'maintenance-tech',
                title: 'Maintenance Technician',
                department: 'Facilities Management',
                salary: '28 credits/hour',
                type: 'maintenance',
                employment: 'part-time',
                requirements: 'Technical repair experience, tool proficiency, physical capability',
                description: 'Perform routine maintenance and emergency repairs on municipal buildings and equipment.'
            },
            {
                id: 'retail-clerk',
                title: 'Retail Sales Associate',
                department: 'Colony General Store',
                salary: '22 credits/hour',
                type: 'retail',
                employment: 'part-time',
                requirements: 'Customer service experience, basic math skills, friendly demeanor',
                description: 'Assist customers with purchases, maintain inventory, and ensure clean store environment.'
            },
            {
                id: 'medical-assistant',
                title: 'Medical Assistant',
                department: 'Colony Health Services',
                salary: '35,000 credits/year',
                type: 'medical',
                employment: 'full-time',
                requirements: 'Medical training certification, patient care experience, attention to detail',
                description: 'Support medical staff with patient care, record keeping, and basic medical procedures.'
            },
            {
                id: 'food-service',
                title: 'Kitchen Staff',
                department: 'Colony Cafeteria',
                salary: '20 credits/hour',
                type: 'food-service',
                employment: 'part-time',
                requirements: 'Food handling certification, team work, ability to work in fast-paced environment',
                description: 'Prepare meals, maintain kitchen cleanliness, and ensure food safety standards.'
            }
        ];
    }
    
    /**
     * Show specific section (contracts or jobs)
     * @param {string} sectionName - Name of section to show
     */
    showSection(sectionName) {
        // Hide all sections
        const sections = document.querySelectorAll('.content-section');
        sections.forEach(section => {
            section.classList.remove('active');
            section.style.display = 'none';
        });
        
        // Show target section
        const targetSection = document.getElementById(sectionName);
        if (targetSection) {
            targetSection.classList.add('active');
            targetSection.style.display = 'block';
        }
        
        // Update navigation
        this.updateNavigation(sectionName);
        
        this.currentSection = sectionName;
        
        console.log(`Contracts section switched to: ${sectionName}`);
    }
    
    /**
     * Update navigation active states
     * @param {string} activeSectionName - Name of currently active section
     */
    updateNavigation(activeSectionName) {
        const navLinks = document.querySelectorAll('.nav-link');
        navLinks.forEach(link => {
            link.classList.remove('active');
            const href = link.getAttribute('href');
            if (href === `#${activeSectionName}`) {
                link.classList.add('active');
            }
        });
    }
    
    /**
     * Filter contracts by type
     * @param {string} filterType - Type to filter by (all, hauling, delivery, repair, municipal)
     */
    filterContracts(filterType) {
        this.currentContractFilter = filterType;
        this.renderContracts();
        
        // Update filter button states
        const filterButtons = document.querySelectorAll('.filter-btn');
        filterButtons.forEach(btn => {
            btn.classList.remove('active');
            if (btn.textContent.toLowerCase() === filterType || 
                (filterType === 'all' && btn.textContent === 'All')) {
                btn.classList.add('active');
            }
        });
        
        console.log(`Contracts filtered by: ${filterType}`);
    }
    
    /**
     * Filter jobs by category
     */
    filterJobs() {
        const filterSelect = document.getElementById('job-filter');
        if (filterSelect) {
            this.currentJobFilter = filterSelect.value;
            this.renderJobs();
            console.log(`Jobs filtered by: ${this.currentJobFilter}`);
        }
    }
    
    /**
     * Render contracts based on current filter
     */
    renderContracts() {
        const container = document.querySelector('.contracts-grid') || document.getElementById('contracts-grid');
        if (!container) return;
        
        const filteredContracts = this.currentContractFilter === 'all' ? 
            this.contracts : 
            this.contracts.filter(contract => contract.type === this.currentContractFilter);
        
        container.innerHTML = filteredContracts.map(contract => `
            <div class="contract-card" data-type="${contract.type}">
                <div class="card-header">
                    <h3>${contract.title}</h3>
                    <span class="difficulty ${contract.difficulty}">${contract.difficulty.charAt(0).toUpperCase() + contract.difficulty.slice(1)}</span>
                </div>
                <div class="card-content">
                    <p><strong>Client:</strong> ${contract.client}</p>
                    <p><strong>Duration:</strong> ${contract.duration}</p>
                    <p><strong>Payment:</strong> ${contract.payment}</p>
                    <p><strong>Description:</strong> ${contract.description}</p>
                </div>
                <div class="card-footer">
                    <button class="btn-accept" onclick="acceptContract('${contract.id}')">Accept Contract</button>
                </div>
            </div>
        `).join('');
    }
    
    /**
     * Render jobs based on current filter
     */
    renderJobs() {
        const container = document.querySelector('.jobs-grid') || document.getElementById('jobs-grid');
        if (!container) return;
        
        const filteredJobs = this.currentJobFilter === 'all' ? 
            this.jobs : 
            this.jobs.filter(job => job.type === this.currentJobFilter);
        
        container.innerHTML = filteredJobs.map(job => `
            <div class="job-card" data-type="${job.type}">
                <div class="card-header">
                    <h3>${job.title}</h3>
                    <span class="job-type ${job.employment}">${job.employment.charAt(0).toUpperCase() + job.employment.slice(1).replace('-', ' ')}</span>
                </div>
                <div class="card-content">
                    <p><strong>Department:</strong> ${job.department}</p>
                    <p><strong>Salary:</strong> ${job.salary}</p>
                    <p><strong>Requirements:</strong> ${job.requirements}</p>
                    <p><strong>Description:</strong> ${job.description}</p>
                </div>
                <div class="card-footer">
                    <button class="btn-apply" onclick="applyForJob('${job.id}')">Apply Now</button>
                </div>
            </div>
        `).join('');
    }
    
    /**
     * Update status bar counts
     */
    updateCounts() {
        const activeContractsCount = document.getElementById('active-contracts-count');
        const jobOpeningsCount = document.getElementById('job-openings-count');
        const acceptedContractsCount = document.getElementById('accepted-contracts-count');
        
        if (activeContractsCount) activeContractsCount.textContent = this.contracts.length;
        if (jobOpeningsCount) jobOpeningsCount.textContent = this.jobs.length;
        if (acceptedContractsCount) acceptedContractsCount.textContent = '0'; // Would track actual accepted contracts
    }
    
    /**
     * Accept a contract (placeholder functionality)
     * @param {string} contractId - ID of contract to accept
     */
    acceptContract(contractId) {
        const contract = this.contracts.find(c => c.id === contractId);
        if (contract) {
            alert(`Contract "${contract.title}" accepted! Please proceed to the municipal office for documentation.`);
            console.log(`Contract accepted: ${contractId}`);
        }
    }
    
    /**
     * Apply for a job (placeholder functionality)
     * @param {string} jobId - ID of job to apply for
     */
    applyForJob(jobId) {
        const job = this.jobs.find(j => j.id === jobId);
        if (job) {
            alert(`Application submitted for "${job.title}"! You will be contacted within 5 business days.`);
            console.log(`Job application submitted: ${jobId}`);
        }
    }
}

// Global functions for HTML event handlers
function showSection(sectionName) {
    if (window.contractsController) {
        window.contractsController.showSection(sectionName);
    }
}

function filterContracts(filterType) {
    if (window.contractsController) {
        window.contractsController.filterContracts(filterType);
    }
}

function filterJobs() {
    if (window.contractsController) {
        window.contractsController.filterJobs();
    }
}

function acceptContract(contractId) {
    if (window.contractsController) {
        window.contractsController.acceptContract(contractId);
    }
}

function applyForJob(jobId) {
    if (window.contractsController) {
        window.contractsController.applyForJob(jobId);
    }
}

// Initialize contracts controller when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    window.contractsController = new ContractsController();
});