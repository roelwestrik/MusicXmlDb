// Helper: Format Date Elegantly
export function formatDate(date: Date, showTime: boolean = false): string {
    const baseFormat = {
        
    }
    if (showTime) {
        return date.toLocaleString("en-US", {
            year: "numeric",
            month: "long",
            day: "numeric",
            hour: "2-digit",
            minute: "2-digit",
            second: "2-digit",
            hour12: false // Ensures 24-hour format
        });
    }
    return date.toLocaleDateString("en-US", {
        year: "numeric",
        month: "long",
        day: "numeric"
    });
}

export function getRelativeTime(date: Date): string {
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffSeconds = Math.floor(diffMs / 1000);
    const diffMinutes = Math.floor(diffSeconds / 60);
    const diffHours = Math.floor(diffMinutes / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffDays > 0) return `Updated ${diffDays} day${diffDays > 1 ? "s" : ""} ago`;
    if (diffHours > 0) return `Updated ${diffHours} hour${diffHours > 1 ? "s" : ""} ago`;
    if (diffMinutes > 0) return `Updated ${diffMinutes} minute${diffMinutes > 1 ? "s" : ""} ago`;
    return "Updated just now";
}
