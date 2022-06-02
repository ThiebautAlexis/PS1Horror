// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
//
// Notes:
//
// ============================================================================ //

using UnityEditor;

namespace EnhancedEditor.Editor
{
    public static partial class ScriptGenerator
    {
        [MenuItem(ScriptCreatorSubMenu + "Horror PS1_Mono", false, MenuItemOrder)]
        public static void CreateHorrorPS1_Mono() => ScriptGeneratorWindow.GetWindow("EnhancedEditor/Editor/ScriptTemplates/HorrorPS1_Mono.txt");
    }
}
