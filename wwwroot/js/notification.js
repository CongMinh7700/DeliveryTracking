// --- Notification Utility ---
function showPushNotification(message, url, icon) {
    Push.create("🚨 Hết tài xế", {
        body: message,
        icon: icon || "/images/driver-warning.png",
        onClick: function () {
            window.focus();
            window.location.href = url || "/DriverStatus";
            this.close();
        }
    });
}

function handleNotificationPermission(message, url, icon) {
    if (!("Notification" in window)) {
        alert("Trình duyệt của bạn không hỗ trợ thông báo.");
        return;
    }
    switch (Notification.permission) {
        case "granted":
            showPushNotification(message, url, icon);
            break;
        case "denied":
            alert("Bạn đã chặn quyền thông báo. Vui lòng vào phần cài đặt trình duyệt để cấp lại quyền cho trang web này.");
            break;
        default:
            Notification.requestPermission().then(function (permission) {
                if (permission === "granted") {
                    showPushNotification(message, url, icon);
                } else {
                    alert("Bạn đã từ chối quyền gửi thông báo");
                }
            });
            break;
    }
}

// --- SignalR Setup ---
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/driverStatusHub")
    .build();

connection.on("ReceiveDriverAlert", function (message) {
    showAlertZone();
    handleNotificationPermission(message);
    loadNotificationDropdown();
});

connection.on("ReceiveDriverStatusList", function (drivers) {
    drivers.forEach(driver => {
        const row = document.getElementById("driver-" + driver.id);
        if (row) {
            const statusCell = row.querySelector(".status");
            if (statusCell) statusCell.textContent = driver.status;
        }
    });
});

connection.start().then(function () {
    console.log("✅ SignalR connected");
    loadNotificationDropdown();
}).catch(function (err) {
    console.error("❌ SignalR error:", err.toString());
});

// --- UI Helpers ---
function showAlertZone() {
    const alertZone = document.getElementById("alertZone");
    const alertTime = document.getElementById("alertTime");
    if (alertZone && alertTime) {
        alertZone.style.display = "block";
        alertTime.innerText = new Date().toLocaleString();
    }
}

function loadNotificationDropdown() {
    fetch('/api/DriverAlert/GetAlerts')
        .then(res => res.json())
        .then(data => {
            const dropdown = document.getElementById("notificationDropdown");
            const badge = document.getElementById("notificationCount");
            dropdown.innerHTML = "";

            if (!data || data.length === 0) {
                dropdown.innerHTML = `<li><span class="dropdown-item text-muted">Không có thông báo</span></li>`;
                badge.style.display = "none";
                return;
            }

            badge.innerText = data.length;
            badge.style.display = "inline";

            data.forEach(alert => {
                const item = document.createElement("li");
                item.innerHTML = `
                    <span class="dropdown-item small text-wrap">
                        🚨 ${alert.message}<br/>
                        <small class="text-muted">${new Date(alert.createdOn).toLocaleString()}</small>
                    </span>
                `;
                dropdown.appendChild(item);
            });
        }).catch(() => {
            // Optional: handle fetch error
        });
}

document.addEventListener("DOMContentLoaded", function () {
    loadNotificationDropdown();
});