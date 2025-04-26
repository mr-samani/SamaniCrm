export interface GetMenuForEditDto {
  id: string;
  file_id: null;
  parent_id: null;
  translations: MenuTranslation[];
}

export interface MenuTranslation {
  lang: string;
  id?: number;
  title: string;
}
