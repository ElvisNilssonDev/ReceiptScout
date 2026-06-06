import { api } from "@/lib/api";
import type {
  CategoryResponse,
  CreateCategoryDto,
  UpdateCategoryDto,
} from "@/lib/types";

export const categoriesApi = {
  list: () => api.get<CategoryResponse[]>("/api/Categories"),
  create: (dto: CreateCategoryDto) => api.post<CategoryResponse>("/api/Categories", dto),
  update: (id: string, dto: UpdateCategoryDto) =>
    api.put<CategoryResponse>(`/api/Categories/${id}`, dto),
  remove: (id: string) => api.del<void>(`/api/Categories/${id}`),
};