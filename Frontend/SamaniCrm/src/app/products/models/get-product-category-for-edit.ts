export interface GetCategoryForEditDto {
  id: string;
  file_id: null;
  parent_id: null;
  translations: ProductCategoryTranslation[];
}

export interface ProductCategoryTranslation {
  lang: string;
  id?: number;
  name: string;
  description: string;
}
