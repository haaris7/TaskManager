// Classname utility - joins classes and filters out falsy values
export const cn = (...classes) => classes.filter(Boolean).join(' ');

// Format date for display
export const formatDate = (dateString) => {
  if (!dateString) return 'N/A';
  return new Date(dateString).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
};