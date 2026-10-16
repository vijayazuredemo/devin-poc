import { useState } from 'react'

function TaskRow({ task, onUpdate, onDelete }) {
  const [isEditing, setIsEditing] = useState(false)
  const [title, setTitle] = useState(task.title)
  const [description, setDescription] = useState(task.description ?? '')

  function startEditing() {
    setTitle(task.title)
    setDescription(task.description ?? '')
    setIsEditing(true)
  }

  async function handleSave() {
    const saved = await onUpdate(task.id, { title, description, isCompleted: task.isCompleted })
    if (saved) {
      setIsEditing(false)
    }
  }

  function handleToggleComplete() {
    onUpdate(task.id, {
      title: task.title,
      description: task.description,
      isCompleted: !task.isCompleted,
    })
  }

  if (isEditing) {
    return (
      <li className="task-row">
        <div className="task-edit">
          <input value={title} maxLength={100} onChange={(e) => setTitle(e.target.value)} />
          <input
            value={description}
            maxLength={500}
            placeholder="Description (optional)"
            onChange={(e) => setDescription(e.target.value)}
          />
        </div>
        <div className="task-actions">
          <button onClick={handleSave}>Save</button>
          <button className="secondary" onClick={() => setIsEditing(false)}>
            Cancel
          </button>
        </div>
      </li>
    )
  }

  return (
    <li className={`task-row ${task.isCompleted ? 'completed' : ''}`}>
      <input
        type="checkbox"
        checked={task.isCompleted}
        onChange={handleToggleComplete}
        aria-label={`Mark "${task.title}" as completed`}
      />
      <div className="task-text">
        <strong>{task.title}</strong>
        {task.description && <p>{task.description}</p>}
        <small>Created {new Date(task.createdAt).toLocaleString()}</small>
      </div>
      <div className="task-actions">
        <button className="secondary" onClick={startEditing}>
          Edit
        </button>
        <button className="danger" onClick={() => onDelete(task.id)}>
          Delete
        </button>
      </div>
    </li>
  )
}

export default TaskRow
