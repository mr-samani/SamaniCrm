import { FileManagetConsts } from './file-manager-consts';

export function setPreviousFolderId(id: string) {
  localStorage.setItem(FileManagetConsts.PreviousFolderIdLocalStorage, id);
}
export function getPreviousFolderId(): string {
  return localStorage.getItem(FileManagetConsts.PreviousFolderIdLocalStorage) || '';
}
export function removePreviousFolderId() {
  return localStorage.removeItem(FileManagetConsts.PreviousFolderIdLocalStorage);
}
