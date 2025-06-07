/* tslint:disable */
/* eslint-disable */
import { UserDTO } from '@shared/service-proxies/model/user-dto';
import { ThumbnailsDto } from '../models/thumbnails-dto';
import { extend } from 'lodash-es';
import { FileNodeDto } from '@shared/service-proxies/model/file-node-dto';

/**
 * description
 */
export class FileManagerDto extends FileNodeDto {
  override children: FileManagerDto[] = [];
  creationTime?: any;
  extension?: any;
  icon?: any;
  id?: any;

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
