namespace AtomicTorch.CBND.CoreMod.Scripts.Bootstrappers
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using AtomicTorch.CBND.CoreMod.Bootstrappers;
    using AtomicTorch.CBND.CoreMod.ClientComponents.Input;
    using AtomicTorch.CBND.CoreMod.UI.Controls.Core;
    using AtomicTorch.CBND.GameApi.Data.Characters;
    using AtomicTorch.CBND.GameApi.Data.World;
    using AtomicTorch.CBND.GameApi.Data;
    using AtomicTorch.CBND.GameApi.Scripting.ClientComponents;
    using AtomicTorch.CBND.GameApi.Scripting;
    using AtomicTorch.CBND.GameApi.ServicesClient;
    using AtomicTorch.CBND.GameApi;

    class BootstrapperClientLines : BaseBootstrapper
    {
        private static ClientInputContext gameplayInputContext;
        private static List<Line> lines = new List<Line>();
        private static readonly IInputClientService Input = Client.Input;

        public override void ClientInitialize()
        {
            BootstrapperClientGame.ResetCallback += BootstrapperClientGame_ResetCallback;
            BootstrapperClientCore.Client.Rendering.PostEffectsRendering += Rendering_PostEffectsRendering;
        }

        private void Rendering_PostEffectsRendering()
        {
            var worldObjects = Client.World.GetWorldObjectsOfProto<IProtoWorldObject>();
            lines.ForEach(l => {
                Client.UI.LayoutRootChildren.Remove(l);
                l = null;
            });
            lines.Clear();

            foreach (var worldObject in worldObjects)
            {
                if (!worldObject.ClientSceneObject.FindComponents<IClientComponent>().Any(obj => obj.GetType().ToString() == "#=ziLLvEceI6Oi9uqFvmkTyqH5fiHwG$gNYqxG4wl7MYGoh"))
                {
                    Client.UI.AttachControl(
                        worldObject,
                        new FormattedTextBlock()
                        {
                            Content = worldObject.ProtoGameObject.Name,
                            FontSize = 32
                        },
                        positionOffset: (0, 0),
                        isFocusable: false);
                }

                if (worldObject.GameObjectType == GameObjectType.Character && worldObject.IsInitialized && !worldObject.IsDestroyed)
                {
                    var character = (ICharacter)worldObject;
                    if (!character.IsCurrentClientCharacter)
                    {
                        var scale = 1 / Client.UI.GetScreenScaleCoefficient();
                        var brush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                        var MyPosition = Input.WorldToScreenPosition(Client.Characters.CurrentPlayerCharacter.Position + (0, Client.Characters.CurrentPlayerCharacter.ProtoCharacter.CharacterWorldWeaponOffsetMelee));
                        var CharPosition = Input.WorldToScreenPosition(character.Position + (0, character.ProtoCharacter.CharacterWorldWeaponOffsetMelee));
                        MyPosition *= scale;
                        CharPosition *= scale;
                        Line line = new Line()
                        {
                            X1 = MyPosition.X,
                            Y1 = MyPosition.Y,
                            X2 = CharPosition.X,
                            Y2 = CharPosition.Y,
                            Stroke = brush,
                            StrokeThickness = 1.0f
                        };
                        Client.UI.LayoutRootChildren.Add(line);
                        lines.Add(line);
                    }
                }
            }
        }

        private void BootstrapperClientGame_ResetCallback()
        {
            gameplayInputContext?.Stop();
            gameplayInputContext = null;
        }
    }
}
