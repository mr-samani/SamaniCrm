export function isVideo(extension?: string) {
  if (!extension) return false;
  return ['mp4', 'wmv', 'flv', 'mpeg', '3gp'].indexOf(extension.toLowerCase()) > -1;
}
