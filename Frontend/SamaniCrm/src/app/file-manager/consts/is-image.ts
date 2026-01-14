export function isImage(extension?: string) {
  if (!extension) return false;
  return ['jpg', 'jpeg', 'png', 'bmp', 'gif', 'tiff', 'hiec', 'webp'].indexOf(extension.toLowerCase()) > -1;
}
