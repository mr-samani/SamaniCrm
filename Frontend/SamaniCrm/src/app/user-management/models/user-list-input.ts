import { PaginationModel } from '@shared/models/pagination.model';

export class UserListInput extends PaginationModel {
  name?: string;
  email?: string;
  constructor(data?: UserListInput) {
    super(data?.page, data?.perPage);
    if (data) {
      for (var property in data) {
        if (data.hasOwnProperty(property)) (<any>this)[property] = (<any>data)[property];
      }
    }
  }
}
