export function guid(): string {
  function s4() {
    return Math.floor((1 + Math.random()) * 0x10000)
      .toString(16)
      .substring(1);
  }

  return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
}

export function generateSequentialGuid(): string {
  // تولید یک GUID جدید
  function generateRandomBytes(): string {
    let bytes = new Uint8Array(16);
    crypto.getRandomValues(bytes);
    return Array.from(bytes, (byte) => byte.toString(16).padStart(2, '0')).join('');
  }

  // بدست آوردن timestamp به صورت بایت
  const timestamp = Date.now().toString(16).padStart(12, '0');

  // تولید یک GUID جدید و حذف خط تیره‌ها
  const randomBytes = generateRandomBytes();

  // ترکیب کردن timestamp و بایت‌های GUID
  const sequentialGuid = timestamp + randomBytes.slice(timestamp.length);

  // تبدیل به قالب استاندارد GUID
  return sequentialGuid.replace(
    /([a-fA-F0-9]{8})([a-fA-F0-9]{4})([a-fA-F0-9]{4})([a-fA-F0-9]{4})([a-fA-F0-9]{12})/,
    '$1-$2-$3-$4-$5',
  );
}

export function generateUniqueId(length = 8): string {
  const chars = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
  let result = '';
  for (let i = 0; i < length; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length));
  }
  return result;
}
