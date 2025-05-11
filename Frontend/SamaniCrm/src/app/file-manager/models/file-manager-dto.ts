/* tslint:disable */
/* eslint-disable */
import { UserDTO } from '@shared/service-proxies/model/user-dto';
import { ThumbnailsDto } from '../models/thumbnails-dto';

/**
 * description
 */
export class FileManagerDto {
  children!: FileManagerDto[];
  creationTime?: any;
  extension?: any;
  filename?: any;
  icon?: any;
  id?: any;
  isDirectory?: any;

  /**
   * for front end
   */
  isOpen?: any;
  lastModifiedTime?: any;
  parentId?: any;
  path?: any;
  size?: any;
  thumbnails?: ThumbnailsDto;
  type?: any;
  user?: UserDTO;
}
