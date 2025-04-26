export interface ProductCategory {
  id: string;
  name: string;
  description: string;
  file_id: string;
  parent_id: string | null;
  children: ProductCategory[];
  file: File;
  active: boolean;
  //
  dragId: string;
}

export interface File {
  thumbnails: Thumbnail[];
  image: string;
}

export interface Thumbnail {
  w: number;
  h: number;
  name: string;
  path: string;
}
