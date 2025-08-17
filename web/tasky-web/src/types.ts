export const TaskyStatus = {
  Todo: 0,
  InProgress: 1,
  Done: 2,
} as const;

export type TaskyStatus = (typeof TaskyStatus)[keyof typeof TaskyStatus];

export const TaskyStatusLabel: Record<TaskyStatus, string> = {
  [TaskyStatus.Todo]: 'Todo',
  [TaskyStatus.InProgress]: 'In Progress',
  [TaskyStatus.Done]: 'Done',
};

export interface Project {
  id: string;
  name: string;
  ownerId: string;
  createdAt: string;
}

export interface TaskDto {
  id: string;
  projectId: string;
  title: string;
  description?: string | null;
  status: TaskyStatus;
  priority: number;
  assigneeId?: string | null;
  dueDate?: string | null;
  createdAt: string;
  updatedAt?: string | null;
}

export interface CommentDto {
  id: string;
  taskId: string;
  authorId: string;
  content: string;
  createdAt: string;
}
