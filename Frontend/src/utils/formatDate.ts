export const formatDate = (dateString: string) =>
  new Intl.DateTimeFormat("fi-FI", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
    timeZone: "Europe/Helsinki",
  }).format(new Date(dateString));

export const formatDateNoHours = (dateString: string) =>
  new Intl.DateTimeFormat("fi-FI", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    timeZone: "Europe/Helsinki",
  }).format(new Date(dateString));

export const getCurrentFormattedDate = () => {
  const today = new Date();
  const year = today.getFullYear();
  const month = String(today.getMonth() + 1).padStart(2, "0"); // January is 0!
  const day = String(today.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
};
