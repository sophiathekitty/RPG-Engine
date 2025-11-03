// Accepted Contracts Dashboard JavaScript

// Load accepted and completed contracts from localStorage
let acceptedContracts = JSON.parse(localStorage.getItem('acceptedContracts') || '[]');
let completedContracts = JSON.parse(localStorage.getItem('completedContracts') || '[]');

// Initialize page
document.addEventListener('DOMContentLoaded', function() {
    loadActiveContracts();
    loadCompletedContracts();
    updateStatusCounts();
    
    // Simulate contract progress for demo purposes
    simulateProgress();
});

// Navigation functions
function showAcceptedSection(sectionName) {
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

// Load active contracts
function loadActiveContracts() {
    const activeGrid = document.getElementById('active-contracts-grid');
    const activeContracts = acceptedContracts.filter(contract => contract.status === 'accepted' || contract.status === 'in-progress');
    
    if (activeContracts.length === 0) {
        activeGrid.innerHTML = `
            <div style="text-align: center; color: var(--text-muted); grid-column: 1 / -1; padding: 3rem;">
                <h3>No Active Contracts</h3>
                <p>You haven't accepted any contracts yet.</p>
                <a href="contracts.html" class="btn-primary" style="display: inline-block; margin-top: 1rem; padding: 1rem 2rem; text-decoration: none;">Browse Available Contracts</a>
            </div>
        `;
        return;
    }
    
    activeGrid.innerHTML = activeContracts.map(contract => createActiveContractCard(contract)).join('');
}

// Load completed contracts
function loadCompletedContracts() {
    const completedGrid = document.getElementById('completed-contracts-grid');
    
    if (completedContracts.length === 0) {
        completedGrid.innerHTML = `
            <div style="text-align: center; color: var(--text-muted); grid-column: 1 / -1; padding: 3rem;">
                <h3>No Completed Contracts</h3>
                <p>Complete some contracts to see your achievement history here.</p>
            </div>
        `;
        return;
    }
    
    completedGrid.innerHTML = completedContracts.map(contract => createCompletedContractCard(contract)).join('');
}

// Create active contract card
function createActiveContractCard(contract) {
    const acceptedDate = new Date(contract.acceptedDate).toLocaleDateString();
    const progress = contract.progress || 0;
    const timeRemaining = calculateTimeRemaining(contract);
    const isOverdue = timeRemaining.overdue;
    
    return `
        <div class="accepted-contract-card ${isOverdue ? 'overdue' : ''}" onclick="openDetailModal('${contract.id}', 'active')">
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
                    <span>Accepted:</span>
                    <span>${acceptedDate}</span>
                </div>
                <div class="info-item">
                    <span>Duration:</span>
                    <span>${contract.duration}</span>
                </div>
                <div class="info-item">
                    <span>Time Remaining:</span>
                    <span class="${isOverdue ? 'overdue-text' : ''}">${timeRemaining.display}</span>
                </div>
            </div>
            <div class="contract-progress">
                <div class="progress-text">Progress: ${progress}%</div>
                <div class="progress-bar">
                    <div class="progress-fill" style="width: ${progress}%"></div>
                </div>
            </div>
            <div class="contract-status">
                <span class="status-badge ${contract.status}">${contract.status.replace('-', ' ').toUpperCase()}</span>
                ${progress >= 100 ? '<span class="ready-badge">READY TO COMPLETE</span>' : ''}
            </div>
        </div>
    `;
}

// Create completed contract card
function createCompletedContractCard(contract) {
    const completedDate = new Date(contract.completedDate).toLocaleDateString();
    const acceptedDate = new Date(contract.acceptedDate).toLocaleDateString();
    
    return `
        <div class="completed-contract-card" onclick="openDetailModal('${contract.id}', 'completed')">
            <div class="contract-header">
                <h3 class="contract-title">${contract.title}</h3>
                <span class="contract-type ${contract.type}">${contract.type}</span>
            </div>
            <div class="contract-info">
                <div class="info-item">
                    <span>Payment Received:</span>
                    <span class="payment-amount">${contract.payment.toLocaleString()} SC</span>
                </div>
                <div class="info-item">
                    <span>Completed:</span>
                    <span>${completedDate}</span>
                </div>
                <div class="info-item">
                    <span>Duration:</span>
                    <span>${contract.duration}</span>
                </div>
                <div class="info-item">
                    <span>Reputation Gained:</span>
                    <span class="reputation-amount">+${contract.reputation || 0}</span>
                </div>
            </div>
            <div class="completion-badge">
                <span class="status-badge completed">COMPLETED</span>
                ${contract.bonus ? `<span class="bonus-badge">BONUS: ${contract.bonus.toLocaleString()} SC</span>` : ''}
            </div>
        </div>
    `;
}

// Calculate time remaining for a contract
function calculateTimeRemaining(contract) {
    const now = new Date();
    const accepted = new Date(contract.acceptedDate);
    const durationMinutes = parseDuration(contract.duration);
    const deadline = new Date(accepted.getTime() + durationMinutes * 60000);
    const timeLeft = deadline - now;
    
    if (timeLeft <= 0) {
        return { display: 'OVERDUE', overdue: true };
    }
    
    const hours = Math.floor(timeLeft / 3600000);
    const minutes = Math.floor((timeLeft % 3600000) / 60000);
    
    if (hours > 0) {
        return { display: `${hours}h ${minutes}m`, overdue: false };
    } else {
        return { display: `${minutes}m`, overdue: false };
    }
}

// Parse duration string to minutes
function parseDuration(durationStr) {
    const parts = durationStr.match(/(\d+)\s*h(?:our)?s?\s*(\d+)?\s*m(?:in)?(?:ute)?s?|(\d+)\s*m(?:in)?(?:ute)?s?/i);
    if (!parts) return 30; // default 30 minutes
    
    if (parts[3]) {
        return parseInt(parts[3]); // only minutes
    } else {
        const hours = parseInt(parts[1]) || 0;
        const minutes = parseInt(parts[2]) || 0;
        return hours * 60 + minutes;
    }
}

// Open detail modal
function openDetailModal(contractId, type) {
    let contract;
    
    if (type === 'active') {
        contract = acceptedContracts.find(c => c.id === contractId);
    } else {
        contract = completedContracts.find(c => c.id === contractId);
    }
    
    if (!contract) return;
    
    const modal = document.getElementById('contract-detail-modal');
    const title = document.getElementById('modal-detail-title');
    const body = document.getElementById('modal-detail-body');
    const completeBtn = document.getElementById('complete-contract-btn');
    const abandonBtn = document.getElementById('abandon-contract-btn');
    
    title.textContent = contract.title;
    
    if (type === 'active') {
        completeBtn.style.display = contract.progress >= 100 ? 'block' : 'none';
        abandonBtn.style.display = 'block';
        completeBtn.setAttribute('data-contract-id', contractId);
        abandonBtn.setAttribute('data-contract-id', contractId);
    } else {
        completeBtn.style.display = 'none';
        abandonBtn.style.display = 'none';
    }
    
    const timeRemaining = type === 'active' ? calculateTimeRemaining(contract) : null;
    const acceptedDate = new Date(contract.acceptedDate).toLocaleDateString();
    const completedDate = contract.completedDate ? new Date(contract.completedDate).toLocaleDateString() : null;
    
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
                    <span>${contract.collateral.toLocaleString()} SC</span>
                </div>
                <div class="info-item">
                    <strong>Distance:</strong>
                    <span>${contract.distance} m</span>
                </div>
                <div class="info-item">
                    <strong>Duration:</strong>
                    <span>${contract.duration}</span>
                </div>
                <div class="info-item">
                    <strong>Accepted Date:</strong>
                    <span>${acceptedDate}</span>
                </div>
                ${completedDate ? `
                <div class="info-item">
                    <strong>Completed Date:</strong>
                    <span>${completedDate}</span>
                </div>
                ` : ''}
                ${timeRemaining ? `
                <div class="info-item">
                    <strong>Time Remaining:</strong>
                    <span class="${timeRemaining.overdue ? 'overdue-text' : ''}">${timeRemaining.display}</span>
                </div>
                ` : ''}
            </div>
            
            <div class="contract-description-full">
                <h4>Description:</h4>
                <p>${contract.description}</p>
            </div>
            
            ${type === 'active' ? `
            <div class="contract-progress-detail">
                <h4>Progress: ${contract.progress || 0}%</h4>
                <div class="progress-bar">
                    <div class="progress-fill" style="width: ${contract.progress || 0}%"></div>
                </div>
                ${contract.progress >= 100 ? '<p style="color: var(--accent-green); font-weight: bold;">✅ Contract ready for completion!</p>' : ''}
            </div>
            ` : ''}
            
            ${contract.bonus ? `
            <div class="bonus-info">
                <h4>Completion Bonus:</h4>
                <p style="color: var(--accent-green);">+${contract.bonus.toLocaleString()} SC bonus received for excellent performance!</p>
            </div>
            ` : ''}
        </div>
    `;
    
    modal.classList.add('show');
    modal.style.display = 'flex';
}

// Close detail modal
function closeDetailModal() {
    const modal = document.getElementById('contract-detail-modal');
    modal.classList.remove('show');
    modal.style.display = 'none';
}

// Complete contract
function completeContract() {
    const contractId = document.getElementById('complete-contract-btn').getAttribute('data-contract-id');
    const contractIndex = acceptedContracts.findIndex(c => c.id === contractId);
    
    if (contractIndex === -1) return;
    
    const contract = acceptedContracts[contractIndex];
    
    // Add completion data
    contract.completedDate = new Date().toISOString();
    contract.status = 'completed';
    contract.progress = 100;
    
    // Random bonus for good performance
    if (Math.random() > 0.7) {
        contract.bonus = Math.floor(contract.payment * 0.1);
    }
    
    // Move to completed contracts
    completedContracts.push(contract);
    acceptedContracts.splice(contractIndex, 1);
    
    // Save to localStorage
    localStorage.setItem('acceptedContracts', JSON.stringify(acceptedContracts));
    localStorage.setItem('completedContracts', JSON.stringify(completedContracts));
    
    // Show completion message
    const bonus = contract.bonus ? ` + ${contract.bonus.toLocaleString()} SC bonus` : '';
    alert(`Contract "${contract.title}" completed successfully!\n\nPayment received: ${contract.payment.toLocaleString()} SC${bonus}\nReputation gained: +${contract.reputation || 0}`);
    
    closeDetailModal();
    loadActiveContracts();
    loadCompletedContracts();
    updateStatusCounts();
}

// Abandon contract
function abandonContract() {
    const contractId = document.getElementById('abandon-contract-btn').getAttribute('data-contract-id');
    const contractIndex = acceptedContracts.findIndex(c => c.id === contractId);
    
    if (contractIndex === -1) return;
    
    const contract = acceptedContracts[contractIndex];
    
    if (!confirm(`Are you sure you want to abandon "${contract.title}"?\n\nThis will result in:\n- Loss of collateral: ${contract.collateral.toLocaleString()} SC\n- Reputation penalty if applicable\n\nThis action cannot be undone.`)) {
        return;
    }
    
    // Remove from accepted contracts
    acceptedContracts.splice(contractIndex, 1);
    
    // Save to localStorage
    localStorage.setItem('acceptedContracts', JSON.stringify(acceptedContracts));
    
    alert(`Contract "${contract.title}" has been abandoned.\nCollateral forfeited: ${contract.collateral.toLocaleString()} SC`);
    
    closeDetailModal();
    loadActiveContracts();
    updateStatusCounts();
}

// Update status counts
function updateStatusCounts() {
    const activeCount = acceptedContracts.length;
    const completedCount = completedContracts.length;
    const totalEarnings = completedContracts.reduce((total, contract) => {
        return total + contract.payment + (contract.bonus || 0);
    }, 0);
    
    document.getElementById('active-count').textContent = activeCount;
    document.getElementById('completed-count').textContent = completedCount;
    document.getElementById('total-earnings').textContent = totalEarnings.toLocaleString();
}

// Simulate contract progress for demo purposes
function simulateProgress() {
    acceptedContracts.forEach(contract => {
        if (!contract.progress) {
            contract.progress = Math.floor(Math.random() * 80) + 10; // Random progress between 10-90%
        } else if (contract.progress < 100 && Math.random() > 0.7) {
            contract.progress = Math.min(100, contract.progress + Math.floor(Math.random() * 20) + 5);
        }
    });
    
    localStorage.setItem('acceptedContracts', JSON.stringify(acceptedContracts));
    
    // Update display every 30 seconds
    setTimeout(() => {
        if (window.location.pathname.includes('accepted-contracts')) {
            simulateProgress();
            loadActiveContracts();
        }
    }, 30000);
}

// Add CSS for additional styling
const additionalStyles = `
<style>
.overdue {
    border-color: var(--accent-red) !important;
    box-shadow: 0 0 15px rgba(231, 76, 60, 0.3) !important;
}

.overdue-text {
    color: var(--accent-red) !important;
    font-weight: bold;
}

.status-badge {
    padding: 0.3rem 0.8rem;
    border-radius: 15px;
    font-size: 0.8rem;
    font-weight: bold;
    text-transform: uppercase;
}

.status-badge.accepted {
    background: var(--primary-blue);
    color: white;
}

.status-badge.in-progress {
    background: var(--accent-orange);
    color: white;
}

.status-badge.completed {
    background: var(--accent-green);
    color: white;
}

.ready-badge {
    background: var(--accent-green);
    color: white;
    padding: 0.3rem 0.8rem;
    border-radius: 15px;
    font-size: 0.7rem;
    font-weight: bold;
    margin-left: 0.5rem;
}

.bonus-badge {
    background: var(--accent-yellow);
    color: black;
    padding: 0.3rem 0.8rem;
    border-radius: 15px;
    font-size: 0.7rem;
    font-weight: bold;
    margin-left: 0.5rem;
}

.completion-badge {
    display: flex;
    align-items: center;
    margin-top: 1rem;
}

.contract-status {
    display: flex;
    align-items: center;
    margin-top: 1rem;
}

.completed-contract-card {
    background: var(--background-light);
    border-radius: 12px;
    padding: 1.5rem;
    border: 2px solid var(--accent-green);
    opacity: 0.9;
    cursor: pointer;
    transition: all 0.3s ease;
}

.completed-contract-card:hover {
    opacity: 1;
    transform: translateY(-3px);
    box-shadow: 0 5px 20px rgba(39, 174, 96, 0.3);
}

.reputation-amount {
    color: var(--accent-yellow);
    font-weight: bold;
}
</style>
`;

document.head.insertAdjacentHTML('beforeend', additionalStyles);

// Close modal when clicking outside
window.onclick = function(event) {
    const modal = document.getElementById('contract-detail-modal');
    if (event.target === modal) {
        closeDetailModal();
    }
}