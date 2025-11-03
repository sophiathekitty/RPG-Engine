// Grindstone Municipal Building Contracts Office JavaScript

// Sample contracts data for Grindstone Municipal Building (Rural Area)
const sampleContracts = [
    {
        id: 'HAULING-001',
        type: 'hauling',
        title: 'Priority Package Delivery',
        description: 'A priority package is to be delivered to a client of ours, and we are looking for the right person for the job. You will pay a mandatory collateral upon accepting this contract. Failing to complete the contract within the time limit will result in our client being quite unhappy with our services, and your collateral will be lost. We are sure you are up to the task.',
        payment: 500000,
        reputation: 0,
        failPenalty: 'None',
        collateral: 10000,
        distance: 1470,
        duration: '30 min',
        status: 'available'
    },
    {
        id: 'MUNICIPAL-001',
        type: 'municipal',
        title: 'Infrastructure Maintenance',
        description: 'Regular maintenance required for municipal building systems including power grid diagnostics, HVAC calibration, and security system updates. Report must be filed with building administration upon completion.',
        payment: 150000,
        reputation: 75,
        failPenalty: '5000 SC',
        collateral: 2500,
        distance: 200,
        duration: '02 h 00 min',
        status: 'available'
    }
];

// Sample jobs data for Grindstone businesses (November 2077 dates - Rural Area)
const sampleJobs = [
    {
        id: 'JOB-001',
        title: 'Cashier/Stock Associate',
        company: 'Quick Stop Market',
        category: 'retail',
        salary: '2400-2800 SC/month',
        description: 'Quick Stop Market is hiring for cashier and stock associate positions. Responsibilities include customer checkout, inventory management, and maintaining store cleanliness. Great entry-level opportunity! Must be comfortable working alone during night shifts in our rural location.',
        requirements: 'Basic math skills, ability to lift 25kg, reliable transportation, willingness to work various shifts including nights',
        contact: 'Stop by Quick Stop Market with resume or apply online',
        posted: new Date('2077-10-30').toISOString()
    },
    {
        id: 'JOB-002',
        title: 'Medical Assistant',
        company: 'Urgent Care - Dr. Rosalie Velasco',
        category: 'medical',
        salary: '3500-4000 SC/month',
        description: 'Dr. Rosalie Velasco\'s Urgent Care clinic is looking for a qualified medical assistant to support patient care, manage appointments, and assist with medical procedures. Join our small but dedicated healthcare team serving the rural Grindstone community!',
        requirements: 'Medical assistant certification required, EMR experience preferred, excellent patient care skills, ability to work flexible hours, must be comfortable in rural medical setting',
        contact: 'Submit resume and certifications to Dr. Velasco\'s clinic',
        posted: new Date('2077-10-22').toISOString()
    }
];

// Accepted contracts storage
let acceptedContracts = JSON.parse(localStorage.getItem('acceptedContracts') || '[]');

// Initialize page
document.addEventListener('DOMContentLoaded', function() {
    loadContracts();
    loadJobs();
    updateStatusCounts();
});

// Navigation functions
function showSection(sectionName) {
    // Hide all sections
    document.querySelectorAll('.content-section').forEach(section => {
        section.classList.remove('active');
    });
    
    // Remove active class from all nav links
    document.querySelectorAll('.nav-link').forEach(link => {
        link.classList.remove('active');
    });
    
    // Show selected section
    document.getElementById(sectionName + '-section').classList.add('active');
    
    // Add active class to clicked nav link
    event.target.classList.add('active');
}

// Load and display contracts
function loadContracts() {
    const contractsGrid = document.getElementById('contracts-grid');
    const allContracts = sampleContracts.filter(contract => contract.status === 'available');
    contractsGrid.innerHTML = allContracts.map(contract => createContractCard(contract)).join('');
}

// Create contract card HTML
function createContractCard(contract) {
    return `
        <div class="contract-card" onclick="openContractModal('${contract.id}')">
            <div class="contract-header">
                <h3 class="contract-title">${contract.title}</h3>
                <span class="contract-type ${contract.type}">${contract.type}</span>
            </div>
            <div class="contract-info">
                <div class="info-item">
                    <span>Payment:</span>
                    <span class="payment-amount">${contract.payment.toLocaleString()} SC</span>
                </div>
                <div class="info-item">
                    <span>Duration:</span>
                    <span>${contract.duration}</span>
                </div>
                <div class="info-item">
                    <span>Distance:</span>
                    <span>${contract.distance} m</span>
                </div>
                <div class="info-item">
                    <span>Collateral:</span>
                    <span>${contract.collateral.toLocaleString()} SC</span>
                </div>
            </div>
            <div class="contract-description">
                ${contract.description.substring(0, 150)}${contract.description.length > 150 ? '...' : ''}
            </div>
        </div>
    `;
}

// Filter contracts
function filterContracts(type) {
    // Update active filter button
    document.querySelectorAll('.filter-btn').forEach(btn => btn.classList.remove('active'));
    event.target.classList.add('active');
    
    const allContracts = sampleContracts.filter(contract => contract.status === 'available');
    const filteredContracts = type === 'all' 
        ? allContracts 
        : allContracts.filter(contract => contract.type === type);
    
    const contractsGrid = document.getElementById('contracts-grid');
    contractsGrid.innerHTML = filteredContracts.map(contract => createContractCard(contract)).join('');
}

// Open contract modal
function openContractModal(contractId) {
    const contract = sampleContracts.find(c => c.id === contractId);
    if (!contract) return;
    
    const modal = document.getElementById('contract-modal');
    const title = document.getElementById('modal-contract-title');
    const body = document.getElementById('modal-contract-body');
    const acceptBtn = document.getElementById('accept-contract-btn');
    
    title.textContent = contract.title;
    acceptBtn.setAttribute('data-contract-id', contractId);
    
    body.innerHTML = `
        <div class="contract-full-details">
            <div class="contract-info-grid">
                <div class="info-item">
                    <strong>Contract Type:</strong>
                    <span class="contract-type ${contract.type}">${contract.type.toUpperCase()}</span>
                </div>
                <div class="info-item">
                    <strong>Payment:</strong>
                    <span class="payment-amount">${contract.payment.toLocaleString()} SC</span>
                </div>
                <div class="info-item">
                    <strong>Reputation Reward:</strong>
                    <span>${contract.reputation || 0}</span>
                </div>
                <div class="info-item">
                    <strong>Fail Penalty:</strong>
                    <span>${contract.failPenalty || 'None'}</span>
                </div>
                <div class="info-item">
                    <strong>Required Collateral:</strong>
                    <span>${contract.collateral} SC</span>
                </div>
                <div class="info-item">
                    <strong>Distance:</strong>
                    <span>${contract.distance} m</span>
                </div>
                <div class="info-item">
                    <strong>Estimated Duration:</strong>
                    <span>${contract.duration}</span>
                </div>
                <div class="info-item">
                    <strong>Status:</strong>
                    <span class="status-${contract.status}">${contract.status.toUpperCase()}</span>
                </div>
            </div>
            
            <div class="contract-description-full">
                <h4>Description:</h4>
                <p>${contract.description}</p>
            </div>
        </div>
    `;
    
    modal.classList.add('show');
    modal.style.display = 'flex';
}

// Close contract modal
function closeContractModal() {
    const modal = document.getElementById('contract-modal');
    modal.classList.remove('show');
    modal.style.display = 'none';
}

// Accept contract
function acceptContract() {
    const contractId = document.getElementById('accept-contract-btn').getAttribute('data-contract-id');
    const contract = sampleContracts.find(c => c.id === contractId);
    
    if (!contract) return;
    
    // Add to accepted contracts with timestamp
    const acceptedContract = {
        ...contract,
        acceptedDate: new Date().toISOString(),
        progress: 0,
        status: 'accepted'
    };
    
    acceptedContracts.push(acceptedContract);
    localStorage.setItem('acceptedContracts', JSON.stringify(acceptedContracts));
    
    // Remove from available contracts
    contract.status = 'accepted';
    
    // Show confirmation
    alert(`Contract "${contract.title}" accepted!\n\nPayment: ${contract.payment.toLocaleString()} SC\nCollateral Required: ${contract.collateral.toLocaleString()} SC\n\nContract details have been added to your accepted contracts.`);
    
    closeContractModal();
    loadContracts(); // Reload to show updated status
    updateStatusCounts();
}

// Load and display jobs
function loadJobs() {
    const jobsGrid = document.getElementById('jobs-grid');
    jobsGrid.innerHTML = sampleJobs.map(job => createJobCard(job)).join('');
}

// Create job card HTML
function createJobCard(job) {
    const postedDate = new Date(job.posted).toLocaleDateString();
    
    return `
        <div class="job-card">
            <div class="job-header">
                <h3 class="job-title">${job.title}</h3>
                <div class="job-company">${job.company}</div>
            </div>
            <div class="job-category">${job.category.replace('-', ' ')}</div>
            <div class="job-salary">${job.salary}</div>
            <div class="job-description">${job.description}</div>
            ${job.requirements ? `
                <div class="job-requirements">
                    <h4>Requirements:</h4>
                    <p>${job.requirements}</p>
                </div>
            ` : ''}
            <div class="job-contact">
                📞 ${job.contact}
            </div>
            <div class="job-posted">Posted: ${postedDate}</div>
        </div>
    `;
}

// Filter jobs
function filterJobs() {
    const filter = document.getElementById('job-filter').value;
    const filteredJobs = filter === 'all' 
        ? sampleJobs 
        : sampleJobs.filter(job => job.category === filter);
    
    const jobsGrid = document.getElementById('jobs-grid');
    jobsGrid.innerHTML = filteredJobs.map(job => createJobCard(job)).join('');
}

// Update status counts
function updateStatusCounts() {
    const activeContracts = sampleContracts.filter(c => c.status === 'available').length;
    const jobOpenings = sampleJobs.length;
    const acceptedContractsCount = acceptedContracts.length;
    
    document.getElementById('active-contracts-count').textContent = activeContracts;
    document.getElementById('job-openings-count').textContent = jobOpenings;
    document.getElementById('accepted-contracts-count').textContent = acceptedContractsCount;
}

// Close modal when clicking outside
window.onclick = function(event) {
    const modal = document.getElementById('contract-modal');
    if (event.target === modal) {
        closeContractModal();
    }
}

// AI Assistant Chatbot Functionality
let isAssistantExpanded = false;

// Contract and job knowledge base
const aiKnowledge = {
    contractTypes: {
        'hauling': {
            name: 'Hauling Contracts',
            description: 'Transport goods, packages, or materials from one location to another. These contracts typically require collateral and have strict time limits. Payment varies based on distance and cargo value.'
        },
        'municipal': {
            name: 'Municipal Contracts',
            description: 'Government-issued contracts for public services like infrastructure maintenance, security patrols, or administrative tasks. Usually offer reputation rewards and steady payment.'
        },
        'repair': {
            name: 'Repair Contracts',
            description: 'Fix damaged equipment, systems, or infrastructure. Requires technical expertise and often involves critical systems that cannot fail.'
        },
        'delivery': {
            name: 'Delivery Contracts',
            description: 'Courier services for documents, small packages, or time-sensitive materials. Generally shorter distance than hauling contracts.'
        }
    },
    jobCategories: {
        'retail': {
            name: 'Retail Jobs',
            description: 'Work in stores, markets, or commercial establishments. Duties include customer service, inventory management, and sales operations.'
        },
        'medical': {
            name: 'Medical Jobs',
            description: 'Healthcare positions in clinics, hospitals, or urgent care facilities. Requires medical training and certifications. Essential for rural communities.'
        },
        'food-service': {
            name: 'Food Service Jobs',
            description: 'Restaurant, café, or food preparation work. Includes servers, cooks, baristas, and food handlers. Often includes tips.'
        },
        'administrative': {
            name: 'Administrative Jobs',
            description: 'Office work, paperwork, scheduling, and organizational tasks. Often involves property management or business operations.'
        },
        'maintenance': {
            name: 'Maintenance Jobs',
            description: 'Repair and upkeep of buildings, equipment, or facilities. Requires technical skills and physical work.'
        }
    },
    cannedResponses: [
        "I'm here to help with contract and job information! Try asking about specific contract types like 'hauling' or job categories like 'medical'.",
        "You can ask me about any of our available contracts or job openings. I know about hauling, municipal, repair, and delivery contracts.",
        "Need help understanding our services? I can explain different job categories: retail, medical, food service, administrative, and maintenance.",
        "I'm your friendly contracts assistant! Ask me anything about our available opportunities or how our system works.",
        "Looking for work? I can tell you about our current job openings or explain what different contract types involve.",
        "Not sure what you're looking for? Try asking about 'contracts', 'jobs', 'payment', or specific categories like 'medical jobs'."
    ]
};

function toggleAssistant() {
    const aiChat = document.getElementById('ai-chat');
    const aiToggle = document.getElementById('ai-toggle');
    const aiStatus = document.getElementById('ai-status');
    
    isAssistantExpanded = !isAssistantExpanded;
    
    if (isAssistantExpanded) {
        aiChat.classList.add('expanded');
        aiToggle.classList.add('expanded');
        aiStatus.textContent = 'Online';
    } else {
        aiChat.classList.remove('expanded');
        aiToggle.classList.remove('expanded');
        aiStatus.textContent = 'Click to chat';
    }
}

function handleAIInput(event) {
    if (event.key === 'Enter') {
        sendMessage();
    }
}

function sendMessage() {
    const input = document.getElementById('ai-input');
    const message = input.value.trim();
    
    if (!message) return;
    
    // Add user message
    addMessage(message, 'user');
    input.value = '';
    
    // Show typing indicator
    showTyping();
    
    // Generate response after a short delay
    setTimeout(() => {
        hideTyping();
        const response = generateResponse(message);
        addMessage(response, 'bot');
    }, 1000 + Math.random() * 1000); // Random delay between 1-2 seconds
}

function addMessage(content, sender) {
    const messagesContainer = document.getElementById('ai-messages');
    const messageDiv = document.createElement('div');
    messageDiv.className = `ai-message ${sender}-message`;
    
    const now = new Date();
    const timeString = now.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    
    messageDiv.innerHTML = `
        <div class="message-content">${content}</div>
        <div class="message-time">${timeString}</div>
    `;
    
    messagesContainer.appendChild(messageDiv);
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
}

function showTyping() {
    const messagesContainer = document.getElementById('ai-messages');
    const typingDiv = document.createElement('div');
    typingDiv.className = 'ai-typing';
    typingDiv.id = 'typing-indicator';
    typingDiv.innerHTML = `
        Assistant is typing
        <div class="ai-typing-dots">
            <span></span>
            <span></span>
            <span></span>
        </div>
    `;
    
    messagesContainer.appendChild(typingDiv);
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
}

function hideTyping() {
    const typingIndicator = document.getElementById('typing-indicator');
    if (typingIndicator) {
        typingIndicator.remove();
    }
}

function generateResponse(message) {
    const lowerMessage = message.toLowerCase();
    
    // Check for contract type keywords
    for (const [key, contract] of Object.entries(aiKnowledge.contractTypes)) {
        if (lowerMessage.includes(key)) {
            return `**${contract.name}**: ${contract.description}`;
        }
    }
    
    // Check for job category keywords
    for (const [key, job] of Object.entries(aiKnowledge.jobCategories)) {
        if (lowerMessage.includes(key)) {
            return `**${job.name}**: ${job.description}`;
        }
    }
    
    // Check for general keywords
    if (lowerMessage.includes('contract') && !lowerMessage.includes('accept')) {
        return "We currently have hauling and municipal contracts available. Hauling contracts involve transportation, while municipal contracts are government work. Would you like details about either?";
    }
    
    if (lowerMessage.includes('job') || lowerMessage.includes('work') || lowerMessage.includes('employ')) {
        return "We have 2 job openings: a Cashier position at Quick Stop Market and a Medical Assistant role at Dr. Velasco's clinic. Both offer competitive rural wages!";
    }
    
    if (lowerMessage.includes('payment') || lowerMessage.includes('money') || lowerMessage.includes('salary')) {
        return "Contract payments range from 150,000 to 500,000 SC depending on complexity. Job salaries range from 2,400 to 4,000 SC per month. Collateral is required for most contracts.";
    }
    
    if (lowerMessage.includes('collateral')) {
        return "Collateral is a security deposit required for contracts. It's returned upon successful completion but forfeited if you fail to complete the contract on time.";
    }
    
    if (lowerMessage.includes('accept') || lowerMessage.includes('apply')) {
        return "To accept a contract, click on it for details and then click 'Accept Contract'. For jobs, follow the contact information provided in each listing.";
    }
    
    if (lowerMessage.includes('rural') || lowerMessage.includes('grindstone')) {
        return "This is the Grindstone Municipal Building Contracts Office, serving our rural community. We have fewer opportunities than downtown but focus on essential services for local residents.";
    }
    
    if (lowerMessage.includes('downtown')) {
        return "For more contract and job opportunities, you can also visit our Downtown Contracts Office which serves the busier central district of Grindstone Colony.";
    }
    
    if (lowerMessage.includes('help') || lowerMessage.includes('how')) {
        return "I can help you understand contract types (hauling, municipal, repair, delivery) and job categories (retail, medical, food service, administrative, maintenance). What would you like to know?";
    }
    
    // Return a random canned response
    const randomIndex = Math.floor(Math.random() * aiKnowledge.cannedResponses.length);
    return aiKnowledge.cannedResponses[randomIndex];
}